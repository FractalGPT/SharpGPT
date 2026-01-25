using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.OpenRouter.ContextFormat.Models;

// Переменные цены
public class VariablePricing
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("threshold")]
    public int Threshold { get; set; }

    [JsonPropertyName("prompt")]
    public string Prompt { get; set; }

    [JsonPropertyName("completions")]
    public string Completions { get; set; }

    [JsonPropertyName("input_cache_read")]
    public string InputCacheRead { get; set; }

    [JsonPropertyName("input_cache_write")]
    public string InputCacheWrite { get; set; }
}
