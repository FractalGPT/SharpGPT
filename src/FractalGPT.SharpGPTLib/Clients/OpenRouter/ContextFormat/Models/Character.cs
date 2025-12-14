using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.OpenRouter.ContextFormat.Models;

// Персонаж (модель AI)
public class Character
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("modelInfo")]
    public ModelInfo ModelInfo { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("includeDefaultSystemPrompt")]
    public bool IncludeDefaultSystemPrompt { get; set; }

    [JsonPropertyName("isStreaming")]
    public bool IsStreaming { get; set; }

    [JsonPropertyName("samplingParameters")]
    public SamplingParameters SamplingParameters { get; set; }

    [JsonPropertyName("maxTokens")]
    public int MaxTokens { get; set; }

    [JsonPropertyName("isDisabled")]
    public bool IsDisabled { get; set; }

    [JsonPropertyName("isRemoved")]
    public bool IsRemoved { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("reasoning")]
    public ReasoningConfig Reasoning { get; set; }

    [JsonPropertyName("plugins")]
    public List<object> Plugins { get; set; }

    [JsonPropertyName("chatMemory")]
    public int ChatMemory { get; set; }
}
