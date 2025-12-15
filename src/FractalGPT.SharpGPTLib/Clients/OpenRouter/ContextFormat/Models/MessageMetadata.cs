using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.OpenRouter.ContextFormat.Models;

// Метаданные сообщения
public class MessageMetadata
{
    [JsonPropertyName("plugins")]
    public List<object> Plugins { get; set; }

    [JsonPropertyName("tokensCount")]
    public int TokensCount { get; set; }

    [JsonPropertyName("variantSlug")]
    public string VariantSlug { get; set; }

    [JsonPropertyName("cost")]
    public string Cost { get; set; }
}
