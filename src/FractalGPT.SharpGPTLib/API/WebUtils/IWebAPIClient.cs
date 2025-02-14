using System.Net.Http.Headers;

namespace FractalGPT.SharpGPTLib.API.WebUtils;


/// <summary>
/// Web API Client, interface for support http client with and without proxy
/// </summary>
public interface IWebAPIClient : IDisposable
{
    AuthenticationHeaderValue Authentication { get; set; }
    Task<HttpResponseMessage> PostAsJsonAsync(string apiUrl, object sendData);
}
