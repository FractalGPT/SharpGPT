using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.API.ChatGPT;

/// <summary>
/// Представляет один из возможных вариантов ответа от модели.
/// </summary>
public class Choice
{
    /// <summary>
    /// Индекс варианта (начиная с 0), обозначающий порядок, в котором модель предложила данный вариант.
    /// </summary>
    [JsonPropertyName("index")]
    public int Index { get; set; }

    /// <summary>
    /// Сообщение, сгенерированное моделью, как часть этого варианта.
    /// </summary>
    [JsonPropertyName("message")]
    public MessageChatGPT Message { get; set; }

    /// <summary>
    /// Причина завершения создания ответа моделью (например, "length" или "stop").
    /// </summary>
    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; }

}
