using System;
using System.Net;

namespace FractalGPT.SharpGPTLib.API.WebUtils;

/// <summary>
/// Provides data for the OnProxyError event.
/// </summary>
[Serializable]
public class ProxyErrorEventArgs : EventArgs
{
    /// <summary>
    /// The proxy server that was attempted for the connection.
    /// </summary>
    public WebProxy Proxy { get; }

    /// <summary>
    /// The exception that occurred as a result of the connection error.
    /// </summary>
    public Exception Exception { get; }

    public ProxyErrorEventArgs(WebProxy proxy, Exception exception)
    {
        Proxy = proxy;
        Exception = exception;
    }
}
