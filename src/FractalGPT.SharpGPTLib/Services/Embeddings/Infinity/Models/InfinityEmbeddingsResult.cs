using AI.DataStructs.Algebraic;
using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Services.Embeddings.Infinity.Models;

/// <summary>
/// Представляет результат запроса эмбеддингов, полученных с использованием модели Infinity.
/// </summary>
public class InfinityEmbeddingsResult
{
    /// <summary>
    /// Коллекция данных эмбеддингов, полученных в результате запроса.
    /// </summary>
    [JsonPropertyName("data")]
    public IEnumerable<InfinityEmbeddingsDataResult> Data { get; set; }
}

/// <summary>
/// Представляет отдельный результат эмбеддинга.
/// </summary>
public class InfinityEmbeddingsDataResult
{
    /// <summary>
    /// Вектор эмбеддинга, представляющий числовое представление входных данных.
    /// </summary>
    [JsonPropertyName("embedding")]
    public Vector Embedding { get; set; }

    /// <summary>
    /// Индекс эмбеддинга, который может использоваться для идентификации или упорядочивания результатов.
    /// </summary>
    [JsonPropertyName("index")]
    public int Index { get; set; }
}
