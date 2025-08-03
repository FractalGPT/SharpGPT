namespace FractalGPT.SharpGPTLib.Stream;
public interface IStreamHandler
{
    Task<string> StartStreamAsync(string streamId, HttpResponseMessage response);

    Task<string> StartStreamAsync(string streamId, string message);

    Task<string> StartStreamAsync<T>(string streamId, T message) where T: class;
}
