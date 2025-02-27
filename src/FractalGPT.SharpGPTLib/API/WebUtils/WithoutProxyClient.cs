﻿using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FractalGPT.SharpGPTLib.API.WebUtils;

[Serializable]
public class WithoutProxyClient : IWebAPIClient
{
    /// <summary>
    /// Property to hold authentication information.
    /// </summary>
    public AuthenticationHeaderValue Authentication { get; set; }

    /// <summary>
    /// Static HttpClient instance reused across requests for better performance.
    /// </summary>
    private static readonly HttpClient HttpClient = new() {
        Timeout = TimeSpan.FromMinutes(10),
    };

    /// <summary>
    /// Constructor to initialize the client with an API key.
    /// </summary>
    /// <param name="apiKey">API key for authentication.</param>
    public WithoutProxyClient(string apiKey)
    {
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
        HttpClient.DefaultRequestHeaders.Authorization = Authentication;
    }

    /// <summary>
    /// Asynchronously sends a POST request with JSON data.
    /// </summary>
    /// <param name="apiUrl">API URL to send the request to.</param>
    /// <param name="sendData">Data to send in the request.</param>
    /// <returns>The HttpResponseMessage from the request.</returns>
    /// <exception cref="HttpRequestException">Thrown when there is an error during the request.</exception>
    public async Task<HttpResponseMessage> PostAsJsonAsync(string apiUrl, object sendData)
    {
        try
        {
            HttpResponseMessage response = await HttpClient.PostAsJsonAsync(apiUrl, sendData);
            _ = response.EnsureSuccessStatusCode();
            return response;
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

    }
}
