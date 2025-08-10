namespace FractalGPT.SharpGPTLib.Stream;
public interface IStreamHandler
{
    Task<string> StartAsync(string streamId, HttpResponseMessage response, string method);

    Task<string> SendAsync(string streamId, string message, string method);

    Task<string> SendAsync<T>(string streamId, T message, string method) where T: class;
}
