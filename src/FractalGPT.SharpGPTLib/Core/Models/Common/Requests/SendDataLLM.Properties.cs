using FractalGPT.SharpGPTLib.Core.Models.Common.Messages;
using FractalGPT.SharpGPTLib.Core.Models.Common.Requests;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.API.LLMAPI;

[Serializable]
public partial class SendDataLLM
{
    #region Основные параметры

    /// <summary>
    /// Получает название модели LLM
    /// </summary>
    [JsonPropertyName("model")]
    public string ModelName { get; }

    /// <summary>
    /// Входные данные
    /// </summary>
    [JsonPropertyName("input")]
    public string Input { get; set; }

    /// <summary>
    /// Получает список сообщений для отправки в LLM
    /// </summary>
    [JsonPropertyName("messages")]
    public List<LLMMessage> Messages { get; protected set; }

    #endregion

    #region Параметры генерации

    /// <summary>
    /// Получает значение температуры для генерации текста (степень случайности)
    /// </summary>
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    /// <summary>
    /// Штраф за повторение токенов
    /// </summary>
    [JsonPropertyName("repetition_penalty")]
    public double RepetitionPenalty { get; set; }

    /// <summary>
    /// Параметр top_p для nucleus sampling
    /// </summary>
    [JsonPropertyName("top_p")]
    public double TopP { get; set; }

    /// <summary>
    /// Параметр top_k - количество наиболее вероятных токенов для выбора
    /// </summary>
    [JsonPropertyName("top_k")]
    public int TopK { get; set; }

    /// <summary>
    /// Минимальное количество токенов в ответе
    /// </summary>
    [JsonPropertyName("min_tokens")]
    public int MinTokens { get; set; }

    /// <summary>
    /// Максимальное количество токенов в ответе
    /// </summary>
    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; }

    #endregion

    #region Параметры потоковой передачи

    /// <summary>
    /// Включить потоковую передачу ответа
    /// </summary>
    [JsonPropertyName("stream")]
    public bool Stream { get; set; }

    /// <summary>
    /// Опции потоковой передачи
    /// </summary>
    [JsonPropertyName("stream_options")]
    public StreamOptions StreamOptions { get; set; }

    // === Параметры вероятностей ===

    /// <summary>
    /// Выводить ли логарифмы вероятностей токенов
    /// </summary>
    [JsonPropertyName("logprobs")]
    public bool? LogProbs { get; set; }

    /// <summary>
    /// Количество наиболее вероятных токенов для отображения вероятностей
    /// </summary>
    [JsonPropertyName("top_logprobs")]
    public int? TopLogprobs { get; set; }

    #endregion

    #region Параметры рассуждений

    /// <summary>
    /// Настройки режима рассуждений
    /// </summary>
    [JsonPropertyName("reasoning")]
    public ReasoningSettings ReasoningSettings { get; set; }

    /// <summary>
    /// Уровень усилий для режима рассуждений
    /// </summary>
    [JsonPropertyName("reasoning_effort")]
    public string ReasoningEffort { get; set; }

    #endregion
}
