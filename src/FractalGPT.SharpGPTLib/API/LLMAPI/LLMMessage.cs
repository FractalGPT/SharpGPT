using FractalGPT.SharpGPTLib.API.LLMAPI.MessageTypes;
using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.API.LLMAPI;

/// <summary>
/// Represents a chat message from different roles (e.g., "user", "assistant").
/// </summary>
[Serializable]
public class LLMMessage
{
    /// <summary>
    /// Gets the role of the message sender (e.g., "user" or "assistant").
    /// </summary>
    [JsonPropertyName("role")]
    public string Role { get; private set; }

    /// <summary>
    /// Gets or sets the content of the message (can be null).
    /// </summary>
    [JsonPropertyName("content")]
    [JsonConverter(typeof(ContentJsonConverter))]
    public object Content { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Message"/> class.
    /// </summary>
    public LLMMessage() // Parameterless constructor for deserialization
    {
        Role = string.Empty;
        Content = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LLMMessage"/> class with the specified role and content.
    /// </summary>
    /// <param name="role">The role of the message sender (e.g., "user", "assistant").</param>
    /// <param name="content">The text content of the message (can be null).</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="role"/> is null or whitespace.</exception>
    public LLMMessage(string role, string content)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be null or whitespace.", nameof(role));

        Role = role;
        Content = content;
    }

    public LLMMessage(string role, MessageContent content)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be null or whitespace.", nameof(role));

        Role = role;
        if (role == "user") Content = content;
        else Content = content.ToString();
    }

    /// <summary>
    /// Creates a message for sending to the LLM API.
    /// </summary>
    /// <param name="role">The role of the sender.</param>
    /// <param name="content">The message content (can be null).</param>
    /// <returns>A new <see cref="LLMMessage"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="role"/> is invalid.</exception>
    public static LLMMessage CreateMessage(Roles role, string content)
    {
        var senderRole = role.ToString().ToLower();
        return new LLMMessage(senderRole, content);
    }

    /// <summary>
    /// Creates a deep copy of the <see cref="LLMMessage"/> instance.
    /// </summary>
    /// <returns>A new <see cref="LLMMessage"/> instance with the same properties.</returns>
    public LLMMessage DeepClone()
    {
        if(Content is string) 
            return new LLMMessage(Role, Content as string);
        else return new LLMMessage(Role, Content as MessageContent);
    }
}
