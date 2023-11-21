using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FractalGPT.SharpGPTLib.API.WebUtils
{

    /// <summary>
    /// Web API Client, interface for support http client with and without proxy
    /// </summary>
    public interface IWebAPIClient : IDisposable
    {
        AuthenticationHeaderValue Authentication { get; set; }
        Task<HttpResponseMessage> PostAsJsonAsync(string apiUrl, object sendData);
    }
}
