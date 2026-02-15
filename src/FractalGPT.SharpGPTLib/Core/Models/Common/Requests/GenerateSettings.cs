namespace FractalGPT.SharpGPTLib.Core.Models.Common.Requests;

/// <summary>
/// Представляет настройки конфигурации для генерации текста.
/// </summary>
public class GenerateSettings
{
    #region Настройки семплирования

    /// <summary>
    /// Температура (0.0-2.0). Выше = креативнее, ниже = сфокусированнее.
    /// </summary>
    public double? Temperature { get; set; }

    /// <summary>
    /// Nucleus sampling (0.0-1.0). Рассматриваются токены с суммарной вероятностью TopP.
    /// </summary>
    public double? TopP { get; set; }

    /// <summary>
    /// Количество топовых токенов для семплирования (> 0).
    /// </summary>
    public int? TopK { get; set; }

    #endregion

    #region Настройки повторений и длины

    /// <summary>
    /// Штраф за повторения (1.0-2.0).
    /// </summary>
    public double? RepetitionPenalty { get; set; }

    /// <summary>
    /// Минимальное количество токенов (≥ 0).
    /// </summary>
    public int? MinTokens { get; set; }

    /// <summary>
    /// Максимальное количество токенов (> 0).
    /// </summary>
    public int? MaxTokens { get; set; }

    #endregion

    #region Настройки логирования

    /// <summary>
    /// Выводить ли логарифмы вероятностей токенов.
    /// </summary>
    public bool? LogProbs { get; set; }

    /// <summary>
    /// Количество топовых логитов для каждого шага (1-20).
    /// </summary>
    public int? TopLogprobs { get; set; }

    #endregion

    #region Настройки потоковой передачи

    public string StreamId { get; }
    public string StreamMethod { get; }

    /// <summary>
    /// Включена ли потоковая передача.
    /// </summary>
    public bool Stream => !string.IsNullOrEmpty(StreamId);

    #endregion

    #region Дополнительные настройки

    /// <summary>
    /// Настройки рассуждений (опционально).
    /// </summary>
    public ReasoningSettings ReasoningSettings { get; set; }

    /// <summary>
    /// Настройки усилий размышления для некоторых моделей, например GoogleAIStudio (Gemini, etc.).
    /// </summary>
    public string ReasoningEffort { get; set; }

    #endregion

    #region Конструктор

    public GenerateSettings(
        double temperature = 0.1,
        double? repetitionPenalty = 1.05,
        double? topP = 0.95,
        int? topK = 20,
        int? minTokens = 8,
        int? maxTokens = 3012,
        string streamId = null,
        string reasoningEffort = null,
        string streamMethod = "StreamMessage")
    {
        Temperature = temperature;
        RepetitionPenalty = repetitionPenalty;
        TopP = topP;
        TopK = topK;
        MinTokens = minTokens;
        MaxTokens = maxTokens;
        StreamId = streamId;
        StreamMethod = streamMethod;
        ReasoningEffort = reasoningEffort;
    }

    #endregion
}