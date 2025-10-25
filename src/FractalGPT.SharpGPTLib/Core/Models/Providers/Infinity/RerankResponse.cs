using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Core.Models.Providers.Infinity;

/// <summary>
/// Класс для десериализации ответа от сервера
/// </summary>
public class RerankResponse
{
    /// <summary>
    /// Тип объекта
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; }

    /// <summary>
    /// Результаты ранжирования
    /// </summary>
    [JsonPropertyName("results")]
    public List<RerankResult> Results { get; set; }

    /// <summary>
    /// Используемая модель
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; }

    /// <summary>
    /// Информация об использовании токенов
    /// </summary>
    [JsonPropertyName("usage")]
    public UsageInfo Usage { get; set; }

    /// <summary>
    /// Уникальный идентификатор запроса
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// Временная метка создания запроса
    /// </summary>
    [JsonPropertyName("created")]
    public long Created { get; set; }
}
