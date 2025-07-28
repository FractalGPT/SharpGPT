using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Encoders.Reranker.VLLM;

public class VLLMRerankRequest
{
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 0;

    /// <summary>
    /// Модель для ранжирования.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; }

    /// <summary>
    /// Запрос, относительно которого ранжируются документы.
    /// </summary>
    [JsonPropertyName("query")]
    public string Query { get; set; }

    /// <summary>
    /// Список документов для ранжирования.
    /// </summary>
    [JsonPropertyName("documents")]
    public IEnumerable<string> Documents { get; set; }
}
