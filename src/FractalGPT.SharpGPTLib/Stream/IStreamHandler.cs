namespace FractalGPT.SharpGPTLib.Stream;
public interface IStreamHandler
{
    Task<string> StartAsync(string streamId, HttpResponseMessage response, string method);

    Task<bool> SendAsync(string streamId, string message, string method);

    Task<bool> SendAsync<T>(string streamId, T message, string method) where T: class;
}
