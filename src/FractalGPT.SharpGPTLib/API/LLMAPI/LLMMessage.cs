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
    public string Role { get; }

    /// <summary>
    /// Gets the content of the message.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LLMMessage"/> class 
    /// for serialization purposes.
    /// </summary>
    private LLMMessage() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LLMMessage"/> class 
    /// with the specified role and content.
    /// </summary>
    /// <param name="role">The role of the message sender (e.g., "user", "assistant").</param>
    /// <param name="content">The text content of the message.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="role"/> or <paramref name="content"/> 
    /// is null or consists solely of whitespace.
    /// </exception>
    public LLMMessage(string role, string content)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be null or whitespace.", nameof(role));

        //Из-за этого периодически падает (когда LLM возвращает пустой ответ)
        if (content == null) //string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be null or whitespace.", nameof(content));

        Role = role;
        Content = content;
    }

    /// <summary>
    /// Создает сообщение для отправки по апи в llm
    /// </summary>
    /// <param name="role"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static LLMMessage CreateMessage(Roles role, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be null or whitespace.", nameof(content));

        var senderRole = role.ToString().ToLower();

        return new LLMMessage(senderRole, content);
    }

    /// <summary>
    /// Метод для глубокого копирования объекта LLMMessage
    /// </summary>
    /// <returns></returns>
    public LLMMessage DeepClone()
    {
        return new LLMMessage(this.Role, this.Content);
    }

}

/// <summary>
/// Роли
/// </summary>
public enum Roles : byte
{
    /// <summary>
    /// Сообщение бота
    /// </summary>
    Assistant = 1,
    /// <summary>
    /// Сообщение пользователя
    /// </summary>
    User = 2,
    /// <summary>
    /// Системное сообщение
    /// </summary>
    System = 3
}
