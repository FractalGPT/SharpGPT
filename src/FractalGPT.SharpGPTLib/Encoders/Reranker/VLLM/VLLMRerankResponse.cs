using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using FractalGPT.SharpGPTLib.Encoders.Reranker.Infinity;

namespace FractalGPT.SharpGPTLib.Encoders.Reranker.VLLM;

public class VLLMRerankResponse
{
    /// <summary>
    /// Тип объекта (error, null)
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; }

    /// <summary>
    /// Используемая модель
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; }

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// Результаты ранжирования
    /// </summary>
    [JsonPropertyName("results")]
    public List<RerankResult> Results { get; set; }
}
