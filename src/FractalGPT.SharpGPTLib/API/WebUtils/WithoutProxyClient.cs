﻿using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace FractalGPT.SharpGPTLib.API.WebUtils;

[Serializable]
public class WithoutProxyClient : IWebAPIClient
{
    /// <summary>
    /// Property to hold authentication information.
    /// </summary>
    public AuthenticationHeaderValue Authentication { get; set; }

    public string ApiKey { get; set; }

    /// <summary>
    /// Static HttpClient instance reused across requests for better performance.
    /// </summary>
    private readonly HttpClient HttpClient = new() {
        Timeout = TimeSpan.FromMinutes(6),
    };

    /// <summary>
    /// Constructor to initialize the client with an API key.
    /// </summary>
    /// <param name="apiKey">API key for authentication.</param>
    public WithoutProxyClient(string apiKey)
    {
        ApiKey = apiKey;
        if (!string.IsNullOrEmpty(apiKey))
            Authentication = new AuthenticationHeaderValue("Bearer", apiKey);
        ConfigureHttpClient();
    }

    public WithoutProxyClient()
    {

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
    /// <returns>The HttpResponseMessage from the request.</returns>
    /// <exception cref="HttpRequestException">Thrown when there is an error during the request.</exception>
    public async Task<HttpResponseMessage> PostAsJsonAsync(string apiUrl, object sendData, CancellationToken? cancellationToken = default)
    {
        cancellationToken ??= new CancellationTokenSource(TimeSpan.FromMinutes(6)).Token;

        if (string.IsNullOrWhiteSpace(apiUrl))
            throw new ArgumentException("apiUrl не может быть пустым.", nameof(apiUrl));
        if (sendData == null)
            throw new ArgumentNullException(nameof(sendData));

        try
        {
            var jsonContent = JsonSerializer.Serialize(sendData, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            });

            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(apiUrl))
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            if (!string.IsNullOrEmpty(ApiKey))
                httpRequestMessage.Headers.TryAddWithoutValidation("Authorization", "Bearer " + ApiKey);

            httpRequestMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
            httpRequestMessage.Headers.TryAddWithoutValidation("X-Version", "1");

            var response = await HttpClient.SendAsync(httpRequestMessage, cancellationToken.Value);
            //_ = response.EnsureSuccessStatusCode();
            return response;
        }
        catch (OperationCanceledException)
        {
            // Пробрасываем исключения отмены без обёртывания
            throw;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException("An error occurred while sending the request", ex);
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
