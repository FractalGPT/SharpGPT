using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.OpenRouter.ContextFormat.Models;

// Цены
public class Pricing
{
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; }

    [JsonPropertyName("completion")]
    public string Completion { get; set; }

    [JsonPropertyName("image")]
    public string Image { get; set; }

    [JsonPropertyName("request")]
    public string Request { get; set; }

    [JsonPropertyName("input_cache_read")]
    public string InputCacheRead { get; set; }

    [JsonPropertyName("input_cache_write")]
    public string InputCacheWrite { get; set; }

    [JsonPropertyName("web_search")]
    public string WebSearch { get; set; }

    [JsonPropertyName("internal_reasoning")]
    public string InternalReasoning { get; set; }

    [JsonPropertyName("image_output")]
    public string ImageOutput { get; set; }

    [JsonPropertyName("discount")]
    public double Discount { get; set; }
}
