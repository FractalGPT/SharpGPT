using System;
using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.API.ChatGPT;

/// <summary>
/// Представляет сообщение в чате от различных ролей (например, пользователя или ассистента).
/// </summary>
[Serializable]
public class MessageChatGPT
{
    /// <summary>
    /// Получает или устанавливает роль отправителя сообщения (например, "user" или "assistant").
    /// </summary>
    [JsonPropertyName("role")]
    public string Role { get; private set; }

    /// <summary>
    /// Получает или устанавливает содержание сообщения.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; private set; }

    /// <summary>
    /// Конструктор по умолчанию для сериализации.
    /// </summary>
    private MessageChatGPT() { }

    /// <summary>
    /// Инициализирует новый экземпляр класса MessageChatGPT с указанными ролью и содержанием.
    /// </summary>
    /// <param name="role">Роль отправителя сообщения.</param>
    /// <param name="content">Содержание сообщения.</param>
    /// <exception cref="ArgumentException">Исключение выбрасывается, если входные данные пусты или состоят только из пробельных символов.</exception>
    public MessageChatGPT(string role, string content)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            throw new ArgumentException("Role cannot be null or whitespace.", nameof(role));
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Content cannot be null or whitespace.", nameof(content));
        }

        Role = role;
        Content = content;
    }
}
