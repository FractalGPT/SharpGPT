using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.API.ChatGPT;

/// <summary>
/// Класс, представляющий данные, отправляемые в ChatGPT, включая сообщения и параметры запроса.
/// </summary>
[Serializable]
public class SendDataChatGPT
{
    private readonly int bufferSize;
    private int currentIndex;

    /// <summary>
    /// Получает или устанавливает название модели ChatGPT.
    /// </summary>
    [JsonPropertyName("model")]
    public string ModelName { get; private set; }

    /// <summary>
    /// Получает или устанавливает значение температуры для генерации текста (степень случайности).
    /// </summary>
    [JsonPropertyName("temperature")]
    public double Temperature { get; private set; }

    /// <summary>
    /// Получает или устанавливает системный промпт, используемый в начале каждого обмена сообщениями.
    /// Не сериализуется, так как передается как часть сообщений.
    /// </summary>
    [JsonIgnore]
    public string Prompt { get; set; }

    /// <summary>
    /// Список сообщений для отправки в ChatGPT.
    /// Хранится в хронологическом порядке с ограничением по размеру.
    /// </summary>
    [JsonPropertyName("messages")]
    public List<MessageChatGPT> Messages { get; private set; }

    /// <summary>
    /// Конструктор инициализирует данные для ChatGPT, устанавливая начальные параметры и первоначальное системное сообщение.
    /// </summary>
    /// <param name="modelName">Название модели ChatGPT.</param>
    /// <param name="systemPrompt">Системный текст, используемый для инициализации общения.</param>
    /// <param name="bufferSize">Максимальный размер списка сообщений.</param>
    /// <param name="temp">Температура для генерации текста.</param>
    public SendDataChatGPT(string modelName, string systemPrompt, int bufferSize = 5, double temp = 0.7)
    {
        ModelName = modelName;
        Temperature = temp;
        Prompt = systemPrompt;
        this.bufferSize = bufferSize;
        Messages = new List<MessageChatGPT>(bufferSize)
        {
            // Инициализация списка сообщений с начальным системным сообщением.
            new MessageChatGPT("system", systemPrompt)
        };
        currentIndex = 1;
    }

    /// <summary>
    /// Добавляет сообщение от пользователя в список сообщений.
    /// </summary>
    /// <param name="text">Текст сообщения пользователя.</param>
    public void AddUserMessage(string text)
    {
        AddMessage("user", text);
    }

    /// <summary>
    /// Добавляет сообщение от ассистента в список сообщений.
    /// </summary>
    /// <param name="text">Текст сообщения ассистента.</param>
    public void AddAssistantMessage(string text)
    {
        AddMessage("assistant", text);
    }

    /// <summary>
    /// Очищает текущий список сообщений, сбрасывая его до начального системного сообщения.
    /// </summary>
    public void Clear()
    {
        Messages.Clear();
        Messages.Add(new MessageChatGPT("system", Prompt));
        currentIndex = 1;
    }

    /// <summary>
    /// Добавляет новое сообщение в список, поддерживая его размер в рамках установленного лимита.
    /// Старые сообщения удаляются при достижении максимального размера.
    /// </summary>
    /// <param name="role">Роль в общении (пользователь или ассистент).</param>
    /// <param name="text">Текст сообщения.</param>
    private void AddMessage(string role, string text)
    {
        // Если достигли размера буфера, удаляем самое старое сообщение
        if (currentIndex >= bufferSize)
            Messages.RemoveAt(0);
        else
            currentIndex++;

        Messages.Add(new MessageChatGPT(role, text));
    }
}
