using FractalGPT.SharpGPTLib.API.LLMAPI;
using FractalGPT.SharpGPTLib.Infrastructure.Extensions;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace FractalGPT.SharpGPTLib.Infrastructure.Http;

[Serializable]
public class ProxyHTTPClient : IWebAPIClient
{
    private readonly ConcurrentBag<ProxyStatus> _proxyStatuses;
    private readonly SemaphoreSlim _semaphore;
    private readonly ProxyHTTPClientOptions _options;
    private readonly TimeSpan _proxyBlacklistDuration = TimeSpan.FromHours(24);
    private readonly int _maxProxyFailures = 8;
    
    // ThreadLocal Random для потокобезопасного случайного выбора прокси
    private static readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() => 
        new Random(Guid.NewGuid().GetHashCode()));
    
    // Кеш HttpClient'ов для каждого прокси (ключ - адрес прокси)
    // ВАЖНО: HttpClient должен жить пока используется response stream!
    private readonly ConcurrentDictionary<string, HttpClient> _httpClientCache = new();
    
    // Статические настройки JSON для переиспользования
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

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
        // Настраиваем TLS один раз при создании
        ConfigureSecurityProtocol();
        
        if (proxies == null)
            throw new ArgumentNullException(nameof(proxies));

        var proxyList = proxies.ToList();
        if (!proxyList.Any())
            throw new ArgumentException("Список прокси не может быть пустым.", nameof(proxies));

        // Валидация прокси
        foreach (var proxy in proxyList)
        {
            if (proxy?.Address == null)
                throw new ArgumentException("Один из прокси имеет некорректный адрес (null).", nameof(proxies));
        }

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

            // Берем только живые прокси и перемешиваем их случайно
            var availableList = _proxyStatuses
                .Where(ps => !IsBlacklisted(ps))
                .ToList();
            
            // Перемешиваем алгоритмом Fisher-Yates для равномерного распределения
            for (int i = availableList.Count - 1; i > 0; i--)
            {
                int j = _random.Value.Next(i + 1);
                var temp = availableList[i];
                availableList[i] = availableList[j];
                availableList[j] = temp;
            }
            
            var availableProxies = availableList.Take(5).ToList();

            if (!availableProxies.Any())
            {
                throw new InvalidOperationException(
                    "Нет доступных прокси. Все прокси в черном списке или отсутствуют.");
            }

            Exception lastException = null;

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

            // Если lastException == null, значит цикл не выполнился (не должно случиться)
            throw lastException ?? new InvalidOperationException(
                "Не удалось выполнить запрос через доступные прокси.");
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
        // Получаем или создаем HttpClient для этого прокси
        var proxyKey = proxy.Address?.ToString() ?? "default";
        var httpClient = _httpClientCache.GetOrAdd(proxyKey, _ => CreateHttpClientForProxy(proxy));

        // Сериализуем данные
        var jsonContent = JsonSerializer.Serialize(sendData, _jsonOptions);

        if (_options.EnableDebugLogging)
        {
            LogDebug($"Отправляемый JSON: {jsonContent.TruncateForLogging()}");
            LogDebug($"URL: {apiUrl}");
            LogDebug($"Proxy: {proxy.Address}");
        }

        // Создаем request и устанавливаем заголовки на НЕГО (потокобезопасно!)
        using var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
        {
            Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json")
        };
        
        // Authorization устанавливаем на request, НЕ на DefaultRequestHeaders!
        if (Authentication != null)
        {
            request.Headers.Authorization = Authentication;
        }

        // Проверяем нужен ли streaming
        var isStreamingRequest = sendData.Stream == true;
        var completionOption = isStreamingRequest 
            ? HttpCompletionOption.ResponseHeadersRead 
            : HttpCompletionOption.ResponseContentRead;

        var response = await httpClient.SendAsync(request, completionOption, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = (await response.Content.ReadAsStringAsync() ?? "").TruncateForLogging();

            if (_options.EnableDebugLogging)
            {
                LogDebug($"Статус ошибки: {response.StatusCode}");
                LogDebug($"Ответ сервера: {errorContent}");
            }

            throw new HttpRequestException($"Ошибка {response.StatusCode}: {errorContent}");
        }

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
    /// Создает HttpClient для конкретного прокси.
    /// HttpClient кешируется и переиспользуется для всех запросов через этот прокси.
    /// </summary>
    private HttpClient CreateHttpClientForProxy(WebProxy proxy)
    {
        var handler = new HttpClientHandler
        {
            Proxy = proxy,
            UseProxy = true,
            AllowAutoRedirect = _options.AllowAutoRedirect,
            UseCookies = _options.UseCookies,
            CookieContainer = _options.Cookie,
            MaxAutomaticRedirections = 3
        };

        // ВНИМАНИЕ: Отключение проверки сертификатов снижает безопасность!
        if (_options.DisableCertificateValidation)
        {
            handler.ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true;
        }

        if (_options.DecompressionMethods.HasValue)
        {
            handler.AutomaticDecompression = _options.DecompressionMethods.Value;
        }

        var httpClient = new HttpClient(handler, disposeHandler: true)
        {
            Timeout = _options.RequestTimeout
        };

        // Используем User Agent из опций или дефолтный
        var userAgent = _options.UserAgent ?? "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";
        httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        return httpClient;
    }

    /// <summary>
    /// Настраивает протоколы безопасности для совместимости с .NET Framework
    /// </summary>
    private static void ConfigureSecurityProtocol()
    {
        try
        {
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 |
                (SecurityProtocolType)0x00000C00 | // Tls13
                SecurityProtocolType.Tls11;
        }
        catch
        {
            // В .NET Core/5+ это может не требоваться, игнорируем
        }
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

        try
        {
            // Создаем Uri для совместимости с .NET Framework 4.8.1
            var proxyUriString = $"http://{proxyData.Address}:{proxyData.Port}";
            var proxyUri = new Uri(proxyUriString);

            var proxy = new WebProxy(proxyUri, BypassOnLocal: false);

            if (!string.IsNullOrEmpty(proxyData.Login) && !string.IsNullOrEmpty(proxyData.Password))
            {
                proxy.UseDefaultCredentials = false;
                proxy.Credentials = new NetworkCredential(
                    userName: proxyData.Login,
                    password: proxyData.Password);
            }

            return proxy;
        }
        catch (UriFormatException ex)
        {
            throw new ArgumentException(
                $"Некорректный формат адреса прокси: {proxyData.Address}:{proxyData.Port}. Ошибка: {ex.Message}", 
                nameof(proxyData), 
                ex);
        }
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

    /// <summary>
    /// Проверяем корректность всех прокси (диагностический метод)
    /// </summary>
    public List<string> ValidateAllProxies()
    {
        var errors = new List<string>();
        
        foreach (var proxyStatus in _proxyStatuses)
        {
            var proxy = proxyStatus.Proxy;
            
            if (proxy == null)
            {
                errors.Add("Найден null прокси в списке");
                continue;
            }

            if (proxy.Address == null)
            {
                errors.Add("Прокси с null адресом");
                continue;
            }

            try
            {
                var uri = proxy.Address;
                if (string.IsNullOrEmpty(uri.Host))
                {
                    errors.Add($"Прокси с пустым хостом: {uri}");
                }
                
                if (uri.Port <= 0)
                {
                    errors.Add($"Прокси с некорректным портом: {uri}");
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Ошибка при проверке прокси {proxy.Address}: {ex.Message}");
            }
        }
        
        return errors;
    }

    public void Dispose()
    {
        _semaphore?.Dispose();
        
        // Dispose все закешированные HttpClient'ы
        foreach (var kvp in _httpClientCache)
        {
            try
            {
                kvp.Value?.Dispose();
            }
            catch
            {
                // Игнорируем ошибки при dispose
            }
        }
        _httpClientCache.Clear();
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
    /// Таймаут на один запрос
    /// </summary>
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromMinutes(7);

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

    /// <summary>
    /// ВНИМАНИЕ: Отключить проверку SSL-сертификатов (небезопасно!)
    /// Используйте только для тестирования или когда абсолютно необходимо.
    /// По умолчанию: false (проверка включена)
    /// </summary>
    public bool DisableCertificateValidation { get; set; } = false;
}

/// <summary>
/// Внутренний класс для отслеживания состояния прокси
/// </summary>
internal class ProxyStatus
{
    private readonly object _lock = new object();
    
    public WebProxy Proxy { get; set; }
    
    private int _failureCount;
    public int FailureCount 
    { 
        get { lock (_lock) return _failureCount; }
        set { lock (_lock) _failureCount = value; }
    }
    
    private DateTime? _lastFailure;
    public DateTime? LastFailure 
    { 
        get { lock (_lock) return _lastFailure; }
        set { lock (_lock) _lastFailure = value; }
    }
    
    private DateTime? _lastSuccess;
    public DateTime? LastSuccess 
    { 
        get { lock (_lock) return _lastSuccess; }
        set { lock (_lock) _lastSuccess = value; }
    }
    
    private Exception _lastException;
    public Exception LastException 
    { 
        get { lock (_lock) return _lastException; }
        set { lock (_lock) _lastException = value; }
    }
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