using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Encoders.Reranker.Infinity;

/// <summary>
/// Класс для результата ранжирования одного документа
/// </summary>
public class RerankResult
{
    /// <summary>
    /// Оценка релевантности документа
    /// </summary>
    [JsonPropertyName("relevance_score")]
    public double RelevanceScore { get; set; }

    /// <summary>
    /// Индекс документа в исходном списке
    /// </summary>
    [JsonPropertyName("index")]
    public int Index { get; set; }

    /// <summary>
    /// Текст документа (может быть null, если сервер не возвращает его)
    /// </summary>
    [JsonPropertyName("document")]
    public string Document { get; set; }
}
