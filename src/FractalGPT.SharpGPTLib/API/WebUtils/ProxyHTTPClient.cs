using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FractalGPT.SharpGPTLib.API.WebUtils
{

    [Serializable]
    public class ProxyHTTPClient : IWebAPIClient
    {
        /// <summary>
        /// List of proxy servers for iteration.
        /// </summary>
        private readonly List<WebProxy> _proxies;

        /// <summary>
        /// Event triggered on a proxy connection error.
        /// </summary>
        public event EventHandler<ProxyErrorEventArgs> OnProxyError;


        public AuthenticationHeaderValue Authentication { get; set; }

        /// <summary>
        /// Initializes a new instance of the ProxyHTTPClient class with a given list of proxies.
        /// </summary>
        /// <param name="proxies">List of proxy servers.</param>
        public ProxyHTTPClient(List<WebProxy> proxies)
        {
            _proxies = proxies ?? throw new ArgumentNullException(nameof(proxies));
        }

        /// <summary>
        /// Initializes a new instance of the ProxyHTTPClient class with a given list of proxies.
        /// </summary>
        /// <param name="proxies">List of proxy servers.</param>
        public ProxyHTTPClient(List<WebProxy> proxies, string apiKey)
        {
            _proxies = proxies ?? throw new ArgumentNullException(nameof(proxies));
            Authentication = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        /// <summary>
        /// Initializes a new instance of the ProxyHTTPClient class
        /// </summary>
        /// <param name="proxyPath">List of proxy servers.</param>
        public ProxyHTTPClient(string proxyPath)
        {
            _proxies = LoadProxiesFromJson(proxyPath);
        }

        public ProxyHTTPClient(string proxyPath, string apiKey)
        {
            _proxies = LoadProxiesFromJson(proxyPath);
            Authentication = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        /// <summary>
        /// Sends an asynchronous HTTP POST request to the specified URL using proxies from the list.
        /// </summary>
        /// <param name="apiUrl">URL to send the request to.</param>
        /// <param name="sendData">Data to be sent in the request.</param>
        /// <returns>HTTP response from the server.</returns>
        /// <exception cref="InvalidOperationException">Thrown if unable to connect through any of the proxy servers.</exception>
        public async Task<HttpResponseMessage> PostAsJsonAsync(string apiUrl, object sendData)
        {
            foreach (var proxy in _proxies)
            {
                try
                {
                    var httpClientHandler = new HttpClientHandler()
                    {
                        Proxy = proxy,
                        UseProxy = true,
                    };

                    using var httpClient = new HttpClient(httpClientHandler);
                    httpClient.DefaultRequestHeaders.Authorization = Authentication;
                    HttpResponseMessage response = await httpClient.PostAsJsonAsync(apiUrl, sendData);
                    _ = response.EnsureSuccessStatusCode();
                    return response;
                }
                catch (Exception ex)
                {
                    OnProxyError?.Invoke(this, new ProxyErrorEventArgs(proxy, ex));
                }
            }

            throw new InvalidOperationException("Failed to connect through any of the proxies.");
        }

        /// <summary>
        /// Load proxy list
        /// </summary>
        /// <param name="jsonFilePath"></param>
        /// <returns></returns>
        public static List<WebProxy> LoadProxiesFromJson(string jsonFilePath)
        {
            var jsonData = File.ReadAllText(jsonFilePath);
            var proxyDataList = JsonSerializer.Deserialize<List<ProxyData>>(jsonData);

            var proxies = new List<WebProxy>();
            foreach (var proxyData in proxyDataList)
                proxies.Add(GetWebProxy(proxyData));

            return proxies;
        }

        /// <summary>
        /// Getting a web proxy
        /// </summary>
        /// <param name="proxyData"></param>
        /// <returns></returns>
        public static WebProxy GetWebProxy(ProxyData proxyData)
        {

            var proxy = new WebProxy($"{proxyData.Address}:{proxyData.Port}")
            {
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    userName: proxyData.Login,
                    password: proxyData.Password)
            };
            return proxy;
        }

        public void Dispose()
        {
            // throw new NotImplementedException();
        }
    }

    [Serializable]
    public class ProxyData
    {
        /// <summary>
        /// Quality or priority of the proxy.
        /// </summary>
        [JsonPropertyName("q")]
        public double Quality { get; set; }

        /// <summary>
        /// Geographical location of the proxy server.
        /// </summary>
        [JsonPropertyName("location")]
        public string Location { get; set; }

        /// <summary>
        /// IP address or hostname of the proxy server.
        /// </summary>
        [JsonPropertyName("ip")]
        public string Address { get; set; }

        /// <summary>
        /// Port number on which the proxy server is listening.
        /// </summary>
        /// <remarks>
        /// Valid port numbers range from 0 to 65535.
        /// </remarks>
        [JsonPropertyName("port")]
        public int Port { get; set; }

        /// <summary>
        /// Login from proxy server
        /// </summary>
        [JsonPropertyName("login")]
        public string Login { get; set; }

        /// <summary>
        /// Password from proxy server
        /// </summary>
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

}
