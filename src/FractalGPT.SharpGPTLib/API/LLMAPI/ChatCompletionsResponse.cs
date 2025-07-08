using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.API.LLMAPI;

/// <summary>
/// Представляет ответ от ChatGPT.
/// </summary>
[Serializable]
public class ChatCompletionsResponse
{
    /// <summary>
    /// Уникальный идентификатор сессии.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// Тип объекта в ответе (обычно "text_completion").
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; }

    /// <summary>
    /// Временная метка создания ответа.
    /// </summary>
    [JsonPropertyName("created")]
    public ulong Created { get; set; }

    /// <summary>
    /// Список вариантов ответов, предоставленных моделью.
    /// </summary>
    [JsonPropertyName("choices")]
    public List<Choice> Choices { get; set; } = new();

    /// <summary>
    /// Информация об использовании, включая количество использованных токенов.
    /// </summary>
    [JsonPropertyName("usage")]
    public Usage Usage { get; set; } = new();

    //public ChatCompletionsResponse(string content)
    //    {
    //        Id = null;
    //        Object = null;
    //        Created = 0;
    //        Choices = new List<Choice>
    //        {
    //            new Choice
    //            {
    //                Index = 0,
    //                Message = new LLMMessage("assistant", content),
    //                FinishReason = null
    //            }
    //        };
    //        Usage = new Usage { };
    //    }
}
