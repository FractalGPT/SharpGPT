using FractalGPT.SharpGPTLib.API.LLMAPI;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace FractalGPT.SharpGPTLib.Infrastructure.Http;

[Serializable]
public class WithoutProxyClient : IWebAPIClient
{
    /// <summary>
    /// Таймаут на получение response headers (защита от молчащего сервера)
    /// После этого таймаута streaming уже должен начаться
    /// </summary>
    private static readonly TimeSpan ResponseHeadersTimeout = TimeSpan.FromSeconds(70);

    /// <summary>
    /// Дефолтный таймаут для LLM запросов (для долгих запросов: o1, reasoning)
    /// </summary>
    private static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromMinutes(18);

    /// <summary>
    /// Property to hold authentication information.
    /// </summary>
    public AuthenticationHeaderValue Authentication { get; set; }

    public string ApiKey { get; set; }

    /// <summary>
    /// HttpClient instance reused across requests for better performance.
    /// </summary>
    private readonly HttpClient HttpClient;

    /// <summary>
    /// Constructor to initialize the client with an API key.
    /// </summary>
    /// <param name="apiKey">API key for authentication.</param>
    public WithoutProxyClient(string apiKey)
    {
        HttpClient = CreateHttpClient();
        
        ApiKey = apiKey;
        if (!string.IsNullOrEmpty(apiKey))
            Authentication = new AuthenticationHeaderValue("Bearer", apiKey);
        ConfigureHttpClient();
    }

    public WithoutProxyClient()
    {
        HttpClient = CreateHttpClient();
    }

    /// <summary>
    /// Создает HttpClient с настроенными таймаутами через SocketsHttpHandler
    /// </summary>
    private static HttpClient CreateHttpClient()
    {
        var handler = new SocketsHttpHandler
        {
            // КРИТИЧНО: Таймаут на установку TCP соединения
            ConnectTimeout = TimeSpan.FromSeconds(60),
            
            // Таймаут ожидания 100-Continue от сервера (для POST с Expect: 100-continue)
            Expect100ContinueTimeout = TimeSpan.FromSeconds(5),
            
            // Таймаут на keep-alive пинг (проверка что соединение живое)
            KeepAlivePingTimeout = TimeSpan.FromSeconds(15),
            KeepAlivePingDelay = TimeSpan.FromSeconds(30),
            KeepAlivePingPolicy = HttpKeepAlivePingPolicy.WithActiveRequests,
            
            // Пул соединений: соединение живёт макс 18 мин, простаивает макс 30 сек
            // (не влияет на активные запросы, только на соединения в пуле)
            PooledConnectionLifetime = DefaultRequestTimeout,
            PooledConnectionIdleTimeout = TimeSpan.FromSeconds(30),
            
            // Ограничение соединений на хост
            MaxConnectionsPerServer = 20,
            
            // Таймаут на получение response после отправки запроса
            // (дополнительная защита на уровне handler)
            ResponseDrainTimeout = TimeSpan.FromSeconds(30),
        };

        return new HttpClient(handler)
        {
            Timeout = DefaultRequestTimeout
        };
    }

    /// <summary>
    /// Method to configure the HttpClient instance. 
    /// Set up HttpClient here, e.g., setting timeouts, proxy settings, etc.
    /// </summary>
    private void ConfigureHttpClient()
    {
        if (Authentication != null)
            HttpClient.DefaultRequestHeaders.Authorization = Authentication;
    }

    /// <summary>
    /// Asynchronously sends a POST request with JSON data.
    /// </summary>
    /// <param name="apiUrl">API URL to send the request to.</param>
    /// <param name="sendData">Data to send in the request.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The HttpResponseMessage from the request.</returns>
    /// <exception cref="HttpRequestException">Thrown when there is an error during the request.</exception>
    public async Task<HttpResponseMessage> PostAsJsonAsync(string apiUrl, SendDataLLM sendData, CancellationToken? cancellationToken = default)
    {
        // КРИТИЧНО: Всегда применяем таймаут для LLM запросов (o1, reasoning)
        // Объединяем с пользовательским токеном через linked token
        var timeoutCts = new CancellationTokenSource(DefaultRequestTimeout);
        CancellationTokenSource timeoutLinkedCts = null;
        
        if (cancellationToken.HasValue)
        {
            // Если пользователь передал токен - объединяем его с таймаутом
            timeoutLinkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken.Value, timeoutCts.Token);
            cancellationToken = timeoutLinkedCts.Token;
        }
        else
        {
            // Если токен не передан - используем только таймаут
            cancellationToken = timeoutCts.Token;
        }

        try
        {
            if (string.IsNullOrWhiteSpace(apiUrl))
                throw new ArgumentException("apiUrl не может быть пустым.", nameof(apiUrl));
            if (sendData == null)
                throw new ArgumentNullException(nameof(sendData));

            var jsonContent = sendData.GetJson(); // Сериализация

            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(apiUrl))
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            if (!string.IsNullOrEmpty(ApiKey))
                httpRequestMessage.Headers.TryAddWithoutValidation("Authorization", "Bearer " + ApiKey);

            httpRequestMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
            httpRequestMessage.Headers.TryAddWithoutValidation("X-Version", "1");

            var isStreamingRequest = (sendData.GetType().GetProperty("Stream")?.GetValue(sendData)) is true;
            var completionOption = isStreamingRequest 
                ? HttpCompletionOption.ResponseHeadersRead 
                : HttpCompletionOption.ResponseContentRead;

            // КРИТИЧНО: Таймаут на получение response headers
            // Защита от молчащего сервера который принял запрос но не отвечает
            using var responseTimeoutCts = new CancellationTokenSource(ResponseHeadersTimeout);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken.Value, responseTimeoutCts.Token);

            HttpResponseMessage response;
            try
            {
                response = await HttpClient.SendAsync(httpRequestMessage, completionOption, linkedCts.Token);
            }
            catch (OperationCanceledException) when (responseTimeoutCts.IsCancellationRequested && !cancellationToken.Value.IsCancellationRequested)
            {
                throw new TimeoutException($"Таймаут ожидания ответа от сервера ({ResponseHeadersTimeout.TotalSeconds} сек). URL: {apiUrl}");
            }

            return response;
        }
        catch (OperationCanceledException)
        {
            // Пробрасываем исключения отмены без обёртывания
            throw;
        }
        catch (TimeoutException)
        {
            // Пробрасываем таймауты без обёртывания
            throw;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException("An error occurred while sending the request", ex);
        }
        finally
        {
            // КРИТИЧНО: Освобождаем CancellationTokenSource'ы которые создали
            timeoutLinkedCts?.Dispose();
            timeoutCts?.Dispose();
        }
    }

    /// <summary>
    /// Asynchronous factory method to create a new WithoutProxyClient instance.
    /// Perform any asynchronous initialization operations here if needed.
    /// </summary>
    /// <param name="apiKey">API key for the client.</param>
    /// <returns>A new instance of WithoutProxyClient.</returns>
    public static async Task<WithoutProxyClient> CreateAsync(string apiKey)
    {
        var client = new WithoutProxyClient(apiKey);
        // Perform any asynchronous initialization here if needed.
        return client;
    }

    public void Dispose()
    {
        HttpClient?.Dispose();
    }
}
