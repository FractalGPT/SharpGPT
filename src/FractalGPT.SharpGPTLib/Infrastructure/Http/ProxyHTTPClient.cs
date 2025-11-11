using FractalGPT.SharpGPTLib.API.LLMAPI;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Infrastructure.Http;

[Serializable]
public class ProxyHTTPClient : IWebAPIClient
{
    private readonly ConcurrentBag<ProxyStatus> _proxyStatuses;
    private readonly SemaphoreSlim _semaphore;
    private readonly ProxyHTTPClientOptions _options;
    private readonly TimeSpan _proxyBlacklistDuration = TimeSpan.FromMinutes(30);
    private readonly int _maxProxyFailures = 10;

    /// <summary>
    /// Срабатывает когда прокси упал
    /// </summary>
    public event EventHandler<ProxyErrorEventArgs> OnProxyError;

    /// <summary>
    /// Срабатывает при отладочных событиях
    /// </summary>
    public event EventHandler<DebugLogEventArgs> OnDebugLog;

    public AuthenticationHeaderValue Authentication { get; set; }

    /// <summary>
    /// Создаем клиент со списком прокси
    /// </summary>
    public ProxyHTTPClient(IEnumerable<WebProxy> proxies, ProxyHTTPClientOptions options = null)
    {
        if (proxies == null)
            throw new ArgumentNullException(nameof(proxies));

        var proxyList = proxies.ToList();
        if (!proxyList.Any())
            throw new ArgumentException("Список прокси не может быть пустым.", nameof(proxies));

        _options = options ?? new ProxyHTTPClientOptions();
        _options.Cookie ??= new CookieContainer();

        _proxyStatuses = new ConcurrentBag<ProxyStatus>(
            proxyList.Select(p => new ProxyStatus { Proxy = p })
        );
        _semaphore = new SemaphoreSlim(_options.MaxConcurrentRequests, _options.MaxConcurrentRequests);
    }

    /// <summary>
    /// Создаем клиент со списком прокси и API ключом
    /// </summary>
    public ProxyHTTPClient(IEnumerable<WebProxy> proxies, string apiKey, ProxyHTTPClientOptions options = null)
        : this(proxies, options)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API ключ не может быть пустым.", nameof(apiKey));

        Authentication = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    /// <summary>
    /// Загружаем прокси из JSON файла
    /// </summary>
    public ProxyHTTPClient(string proxyPath, ProxyHTTPClientOptions options = null)
        : this(LoadProxiesFromJson(proxyPath), options)
    {
    }

    /// <summary>
    /// Загружаем прокси из файла и сразу добавляем API ключ
    /// </summary>
    public ProxyHTTPClient(string proxyPath, string apiKey, ProxyHTTPClientOptions options = null)
        : this(LoadProxiesFromJson(proxyPath), apiKey, options)
    {
    }

    /// <summary>
    /// Отправляем POST запрос через рабочий прокси
    /// </summary>
    public async Task<HttpResponseMessage> PostAsJsonAsync(
        string apiUrl,
        SendDataLLM sendData,
        CancellationToken? cancellationToken = null)
    {
        if (string.IsNullOrWhiteSpace(apiUrl))
            throw new ArgumentException("apiUrl не может быть пустым.", nameof(apiUrl));
        if (sendData == null)
            throw new ArgumentNullException(nameof(sendData));

        var effectiveToken = cancellationToken ?? CancellationToken.None;

        await _semaphore.WaitAsync(effectiveToken);
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(effectiveToken);
            cts.CancelAfter(_options.GlobalTimeout);

            // Берем только живые прокси, сначала те что реже падали
            var availableProxies = _proxyStatuses
                .Where(ps => !IsBlacklisted(ps))
                .OrderBy(ps => ps.FailureCount)
                .ToList();

            if (!availableProxies.Any())
            {
                throw new InvalidOperationException(
                    "Нет доступных прокси. Все прокси в черном списке или отсутствуют.");
            }

            Exception lastException = new();

            foreach (var proxyStatus in availableProxies)
            {
                try
                {
                    var response = await SendRequestThroughProxy(
                        apiUrl,
                        sendData,
                        proxyStatus.Proxy,
                        cts.Token);

                    MarkProxySuccess(proxyStatus);
                    return response;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    MarkProxyFailure(proxyStatus, ex);
                    OnProxyError?.Invoke(this, new ProxyErrorEventArgs(proxyStatus.Proxy, ex));
                    lastException = ex;
                }
            }

            throw lastException;

            // throw new InvalidOperationException("Не удалось подключиться через ни один из доступных прокси.");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Отправляем запрос через конкретный прокси
    /// </summary>
    private async Task<HttpResponseMessage> SendRequestThroughProxy(
        string apiUrl,
        SendDataLLM sendData,
        WebProxy proxy,
        CancellationToken cancellationToken)
    {
        var handler = new HttpClientHandler
        {
            Proxy = proxy,
            UseProxy = true,
            AllowAutoRedirect = _options.AllowAutoRedirect,
            UseCookies = _options.UseCookies,
            CookieContainer = _options.Cookie,
            MaxAutomaticRedirections = 3,
            ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true
        };

        if (_options.DecompressionMethods.HasValue)
        {
            handler.AutomaticDecompression = _options.DecompressionMethods.Value;
        }

        using var httpClient = new HttpClient(handler, disposeHandler: true)
        {
            Timeout = TimeSpan.FromSeconds(_options.RequestTimeout)
        };

        if (Authentication != null)
        {
            httpClient.DefaultRequestHeaders.Authorization = Authentication;
        }

        // Используем User Agent из опций или дефолтный
        var userAgent = _options.UserAgent ?? "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";
        httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        // Явная сериализация с настройками
        var jsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        var jsonContent = JsonSerializer.Serialize(sendData, jsonOptions);

        if (_options.EnableDebugLogging)
        {
            LogDebug($"Отправляемый JSON: {jsonContent}");
            LogDebug($"URL: {apiUrl}");
            LogDebug($"Proxy: {proxy.Address}");
        }

        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(apiUrl, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();

            if (_options.EnableDebugLogging)
            {
                LogDebug($"Статус ошибки: {response.StatusCode}");
                LogDebug($"Ответ сервера: {errorContent}");
            }

            throw new HttpRequestException($"Ошибка {response.StatusCode}: {errorContent}");
        }

        response.EnsureSuccessStatusCode();
        return response;
    }

    /// <summary>
    /// Проверяем не забанен ли прокси
    /// </summary>
    private bool IsBlacklisted(ProxyStatus proxyStatus)
    {
        if (proxyStatus.FailureCount < _maxProxyFailures)
            return false;

        if (proxyStatus.LastFailure == null)
            return false;

        var lastFailureDt = proxyStatus.LastFailure.Value;
        var blacklistExpiration = lastFailureDt.Add(_proxyBlacklistDuration);

        if (DateTime.UtcNow >= blacklistExpiration)
        {
            // Время вышло, даем прокси второй шанс, но с меньшим приоритетом
            // Сбрасываем счетчик
            proxyStatus.FailureCount = 0;
            proxyStatus.LastFailure = null;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Прокси молодец, полностью сбрасываем счетчик косяков
    /// </summary>
    private void MarkProxySuccess(ProxyStatus proxyStatus)
    {
        // При любом успешном запросе полностью сбрасываем счетчик ошибок
        // Если прокси работает - значит все прошлые проблемы неактуальны
        proxyStatus.FailureCount = 0;
        proxyStatus.LastFailure = null;
        proxyStatus.LastSuccess = DateTime.UtcNow;
        proxyStatus.LastException = null;
    }

    /// <summary>
    /// Прокси накосячил, помечаем это
    /// </summary>
    private void MarkProxyFailure(ProxyStatus proxyStatus, Exception exception)
    {
        proxyStatus.FailureCount++;
        proxyStatus.LastFailure = DateTime.UtcNow;
        proxyStatus.LastException = exception;
    }

    /// <summary>
    /// Логируем отладочную информацию через событие
    /// </summary>
    private void LogDebug(string message)
    {
        OnDebugLog?.Invoke(this, new DebugLogEventArgs(message));
    }

    /// <summary>
    /// Читаем список прокси из JSON файла
    /// </summary>
    public static List<WebProxy> LoadProxiesFromJson(string jsonFilePath)
    {
        if (string.IsNullOrWhiteSpace(jsonFilePath))
            throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(jsonFilePath));

        if (!File.Exists(jsonFilePath))
            throw new FileNotFoundException($"Файл не найден: {jsonFilePath}");

        try
        {
            var jsonData = File.ReadAllText(jsonFilePath);
            var proxyDataList = JsonSerializer.Deserialize<List<ProxyData>>(jsonData);

            if (proxyDataList == null || !proxyDataList.Any())
                throw new InvalidOperationException("Файл не содержит данных о прокси.");

            return proxyDataList.Select(GetWebProxy).ToList();
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"Ошибка при разборе JSON файла: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Собираем WebProxy из данных
    /// </summary>
    public static WebProxy GetWebProxy(ProxyData proxyData)
    {
        if (proxyData == null)
            throw new ArgumentNullException(nameof(proxyData));

        if (string.IsNullOrWhiteSpace(proxyData.Address))
            throw new ArgumentException("Адрес прокси не может быть пустым.");

        if (proxyData.Port <= 0 || proxyData.Port > 65535)
            throw new ArgumentException($"Некорректный порт: {proxyData.Port}");

        var proxyAddress = $"{proxyData.Address}:{proxyData.Port}";

        var proxy = !string.IsNullOrEmpty(proxyData.Login) && !string.IsNullOrEmpty(proxyData.Password)
            ? new WebProxy(proxyAddress)
            {
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    userName: proxyData.Login,
                    password: proxyData.Password)
            }
            : new WebProxy(proxyAddress);

        return proxy;
    }

    /// <summary>
    /// Показываем статистику по всем прокси
    /// </summary>
    public IEnumerable<ProxyStatistics> GetProxyStatistics()
    {
        return _proxyStatuses.Select(ps => new ProxyStatistics
        {
            ProxyAddress = $"{ps.Proxy.Address}",
            FailureCount = ps.FailureCount,
            LastSuccess = ps.LastSuccess,
            LastFailure = ps.LastFailure,
            IsBlacklisted = IsBlacklisted(ps),
            LastException = ps.LastException?.Message
        }).ToList();
    }

    /// <summary>
    /// Обнуляем всю статистику и даем всем прокси чистый лист
    /// </summary>
    public void ResetProxyStatistics()
    {
        foreach (var proxyStatus in _proxyStatuses)
        {
            proxyStatus.FailureCount = 0;
            proxyStatus.LastFailure = null;
            proxyStatus.LastSuccess = null;
            proxyStatus.LastException = null;
        }
    }

    public void Dispose()
    {
        _semaphore?.Dispose();
    }
}

/// <summary>
/// Настройки для HTTP клиента с прокси
/// </summary>
public class ProxyHTTPClientOptions
{
    /// <summary>
    /// Разрешить автоматические редиректы
    /// </summary>
    public bool AllowAutoRedirect { get; set; } = true;

    /// <summary>
    /// Использовать куки
    /// </summary>
    public bool UseCookies { get; set; } = false;

    /// <summary>
    /// Контейнер для кук
    /// </summary>
    public CookieContainer Cookie { get; set; }

    /// <summary>
    /// Методы декомпрессии
    /// </summary>
    public DecompressionMethods? DecompressionMethods { get; set; } =
        System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;

    /// <summary>
    /// User Agent для запросов
    /// </summary>
    public string UserAgent { get; set; }

    /// <summary>
    /// Таймаут на один запрос в секундах
    /// </summary>
    public int RequestTimeout { get; set; } = 500;

    /// <summary>
    /// Глобальный таймаут для всех попыток
    /// </summary>
    public TimeSpan GlobalTimeout { get; set; } = TimeSpan.FromMinutes(20);

    /// <summary>
    /// Максимум одновременных запросов
    /// </summary>
    public int MaxConcurrentRequests { get; set; } = 5;

    /// <summary>
    /// Включить отладочное логирование через события
    /// </summary>
    public bool EnableDebugLogging { get; set; } = false;
}

/// <summary>
/// Внутренний класс для отслеживания состояния прокси
/// </summary>
internal class ProxyStatus
{
    public WebProxy Proxy { get; set; }
    public int FailureCount { get; set; }
    public DateTime? LastFailure { get; set; }
    public DateTime? LastSuccess { get; set; }
    public Exception LastException { get; set; }
}

/// <summary>
/// Статистика по прокси для внешнего использования
/// </summary>
public class ProxyStatistics
{
    public string ProxyAddress { get; set; }
    public int FailureCount { get; set; }
    public DateTime? LastSuccess { get; set; }
    public DateTime? LastFailure { get; set; }
    public bool IsBlacklisted { get; set; }
    public string LastException { get; set; }
}

[Serializable]
public class ProxyData
{
    /// <summary>
    /// Качество или приоритет прокси
    /// </summary>
    [JsonPropertyName("q")]
    public double Quality { get; set; }

    /// <summary>
    /// Где находится прокси-сервер
    /// </summary>
    [JsonPropertyName("location")]
    public string Location { get; set; }

    /// <summary>
    /// IP адрес или домен прокси
    /// </summary>
    [JsonPropertyName("ip")]
    public string Address { get; set; }

    /// <summary>
    /// Порт на котором слушает прокси (от 0 до 65535)
    /// </summary>
    [JsonPropertyName("port")]
    public int Port { get; set; }

    /// <summary>
    /// Логин для авторизации на прокси
    /// </summary>
    [JsonPropertyName("login")]
    public string Login { get; set; }

    /// <summary>
    /// Пароль для авторизации на прокси
    /// </summary>
    [JsonPropertyName("password")]
    public string Password { get; set; }
}


/// <summary>
/// Данные отладочного лога для события
/// </summary>
public class DebugLogEventArgs : EventArgs
{
    public string Message { get; }
    public DateTime Timestamp { get; }

    public DebugLogEventArgs(string message)
    {
        Message = message;
        Timestamp = DateTime.UtcNow;
    }
}