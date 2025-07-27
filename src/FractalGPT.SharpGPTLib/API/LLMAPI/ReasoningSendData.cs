using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.API.LLMAPI;

/// <summary>
/// Represents the reasoning configuration for a chat completion response.
/// </summary>
[Serializable]
public class ReasoningSettings
{
    /// <summary>
    /// Gets or sets the effort level for reasoning (e.g., "high", "medium", "low"). Mutually exclusive with MaxTokens.
    /// </summary>
    [JsonPropertyName("effort")]
    public string Effort { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of tokens for reasoning. Mutually exclusive with Effort.
    /// </summary>
    [JsonPropertyName("max_tokens")]
    public int? MaxTokens { get; set; }

    /// <summary>
    /// Gets or sets whether reasoning tokens are excluded from the response. Default is false.
    /// </summary>
    [JsonPropertyName("exclude")]
    public bool Exclude { get; set; } = false;

    /// <summary>
    /// Gets or sets whether reasoning is enabled. Inferred from Effort or MaxTokens if not specified.
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Reasoning"/> class for serialization.
    /// </summary>
    public ReasoningSettings() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Reasoning"/> class with specified parameters.
    /// </summary>
    /// <param name="effort">The effort level (e.g., "high", "medium", "low").</param>
    /// <param name="maxTokens">The maximum number of tokens for reasoning.</param>
    /// <param name="exclude">Whether to exclude reasoning tokens.</param>
    /// <param name="enabled">Whether reasoning is enabled.</param>
    /// <exception cref="ArgumentException">Thrown when both <paramref name="effort"/> and <paramref name="maxTokens"/> are provided.</exception>
    public ReasoningSettings(string effort = null, int? maxTokens = null, bool exclude = false, bool enabled = false)
    {
        if (!string.IsNullOrEmpty(effort) && maxTokens.HasValue)
            throw new ArgumentException("Effort and MaxTokens cannot both be specified.");

        Effort = effort;
        MaxTokens = maxTokens;
        Exclude = exclude;
        Enabled = enabled || !string.IsNullOrEmpty(effort) || maxTokens.HasValue;
    }
}
