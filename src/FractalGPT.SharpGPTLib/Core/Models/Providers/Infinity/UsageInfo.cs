using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Core.Models.Providers.Infinity;

/// <summary>
/// Класс для информации об использовании токенов
/// </summary>
public class UsageInfo
{
    /// <summary>
    /// Количество токенов, использованных в запросе
    /// </summary>
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }

    /// <summary>
    /// Общее количество токенов
    /// </summary>
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}
