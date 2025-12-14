using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.OpenRouter.ContextFormat.Models;

public class Icon
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}
