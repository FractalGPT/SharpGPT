using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.OpenRouter.ContextFormat.Models;

// Данные reasoning
public class ReasoningItemData
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("content")]
    public List<ReasoningContent> Content { get; set; }

    [JsonPropertyName("summary")]
    public List<object> Summary { get; set; }
}
