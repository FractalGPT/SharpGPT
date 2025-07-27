using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.API.LLMAPI;


/// <summary>
/// Represents token usage information for a given request or session.
/// </summary>
[Serializable]
public class Usage
{
    /// <summary>
    /// Gets or sets the number of tokens used in the prompt.
    /// </summary>
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }

    /// <summary>
    /// Gets or sets the number of tokens used by the model's completion.
    /// </summary>
    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }

    /// <summary>
    /// Gets or sets the total number of tokens used in the session.
    /// </summary>
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}

