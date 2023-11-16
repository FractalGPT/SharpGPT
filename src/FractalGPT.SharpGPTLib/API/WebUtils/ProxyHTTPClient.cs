using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace FractalGPT.SharpGPTLib.API.WebUtils
{


    [Serializable]
    public class ProxyHTTPClient
    {
        /// <summary>
        /// List of proxy servers for iteration.
        /// </summary>
        private List<WebProxy> _proxies;

        /// <summary>
        /// Event triggered on a proxy connection error.
        /// </summary>
        public event EventHandler<ProxyErrorEventArgs> OnProxyError;

        /// <summary>
        /// Initializes a new instance of the ProxyHTTPClient class with a given list of proxies.
        /// </summary>
        /// <param name="proxies">List of proxy servers.</param>
        public ProxyHTTPClient(List<WebProxy> proxies)
        {
            _proxies = proxies ?? throw new ArgumentNullException(nameof(proxies));
        }

        /// <summary>
        /// Initializes a new instance of the ProxyHTTPClient class
        /// </summary>
        /// <param name="proxyPath">List of proxy servers.</param>
        public ProxyHTTPClient(string proxyPath)
        {
            _proxies = LoadProxiesFromJson(proxyPath);
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
                        UseProxy = true
                    };

                    using (var httpClient = new HttpClient(httpClientHandler))
                    {
                        HttpResponseMessage response = await httpClient.PostAsJsonAsync(apiUrl, sendData);
                        response.EnsureSuccessStatusCode();
                        return response;
                    }
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
            {
                proxies.Add(new WebProxy(proxyData.Address, proxyData.Port));
            }

            return proxies;
        }
    }

    [Serializable]
    public class ProxyData
    {
        public string Address { get; set; }
        public int Port { get; set; }
    }

}
