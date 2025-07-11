namespace FractalGPT.SharpGPTLib.Stream;
public interface IStreamHandler
{
    Task<string> StartStreamAsync(string streamId, HttpResponseMessage response);
}
