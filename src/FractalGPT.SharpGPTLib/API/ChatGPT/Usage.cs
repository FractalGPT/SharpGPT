using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.API.ChatGPT;

/// <summary>
/// Содержит информацию об использовании токенов в текущем запросе.
/// </summary>
public class Usage
{
    /// <summary>
    /// Количество токенов, использованных для промпта.
    /// </summary>
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }

    /// <summary>
    /// Количество токенов, использованных для ответа модели.
    /// </summary>
    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }

    /// <summary>
    /// Общее количество использованных токенов за сессию.
    /// </summary>
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }

}