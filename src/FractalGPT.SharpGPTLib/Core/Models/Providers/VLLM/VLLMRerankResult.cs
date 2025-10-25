using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Core.Models.Providers.VLLM;

public class VLLMRerankResult
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
}
