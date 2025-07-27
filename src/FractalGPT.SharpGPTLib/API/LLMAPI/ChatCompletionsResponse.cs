using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.API.LLMAPI;

/// <summary>
/// Represents a response from a chat completion API.
/// </summary>
[Serializable]
public class ChatCompletionsResponse
{
    /// <summary>
    /// Unique identifier for the session.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// The provider of the model
    /// </summary>
    [JsonPropertyName("provider")]
    public string Provider { get; set; }

    /// <summary>
    /// The specific model used
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; }

    /// <summary>
    /// Type of object in the response (e.g., "chat.completion").
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; }

    /// <summary>
    /// Timestamp of response creation.
    /// </summary>
    [JsonPropertyName("created")]
    public ulong Created { get; set; }

    /// <summary>
    /// List of response options provided by the model.
    /// </summary>
    [JsonPropertyName("choices")]
    public List<Choice> Choices { get; set; } = new();

    /// <summary>
    /// Information about token usage.
    /// </summary>
    [JsonPropertyName("usage")]
    public Usage Usage { get; set; } = new();
}
