namespace FractalGPT.SharpGPTLib.Stream;
public interface IStreamHandler
{
    Task<string> StartStreamAsync(string streamId, string question, HttpResponseMessage response);
}
