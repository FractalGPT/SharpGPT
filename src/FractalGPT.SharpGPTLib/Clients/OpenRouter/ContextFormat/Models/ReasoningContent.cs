using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.OpenRouter.ContextFormat.Models;

public class ReasoningContent
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }
}
