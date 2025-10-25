using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Services.Embeddings.Infinity.Models;

/// <summary>
/// Параметры запроса для генерации эмбеддингов с использованием модели Infinity.
/// </summary>
public class InfinityEmbeddingsArgs
{
    /// <summary>
    /// Название модели, которая будет использоваться для генерации эмбеддингов.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; }

    /// <summary>
    /// Коллекция входных текстов, для которых необходимо сгенерировать эмбеддинги.
    /// </summary>
    [JsonPropertyName("input")]
    public IEnumerable<string> Input { get; set; }
}

