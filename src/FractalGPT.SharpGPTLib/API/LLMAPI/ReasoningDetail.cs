using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.API.LLMAPI;

/// <summary>
/// Represents a single reasoning detail entry.
/// </summary>
[Serializable]
public class ReasoningDetail
{
    /// <summary>
    /// Type of reasoning detail (e.g., "reasoning.text").
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// Text content of the reasoning detail.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; }

    /// <summary>
    /// Format of the reasoning detail (e.g., "unknown").
    /// </summary>
    [JsonPropertyName("format")]
    public string Format { get; set; }
}