using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Encoders.Reranker.Infinity;

/// <summary>
/// Класс для тела запроса на ранжирование.
/// </summary>
public class RerankRequest
{
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
    public List<string> Documents { get; set; }
}
