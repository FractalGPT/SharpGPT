using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Core.Models.Common.Requests;

/// <summary>
/// Настройки рассуждений для генерации ответов с расширенным мышлением (CoT, ToT и подобные)
/// </summary>
[Serializable]
public class ReasoningSettings
{
    #region Константы уровней рассуждения (усилий)

    /// <summary>Высокий уровень усилий для сложных задач.</summary>
    public const string EffortHigh = "high";

    /// <summary>Средний уровень усилий (по умолчанию).</summary>
    public const string EffortMedium = "medium";

    /// <summary>Низкий уровень усилий для простых задач.</summary>
    public const string EffortLow = "low";

    #endregion

    #region Основные настройки

    /// <summary>
    /// Уровень усилий для рассуждений: "high", "medium" или "low".
    /// Взаимоисключающее с <see cref="MaxTokens"/>.
    /// </summary>
    [JsonPropertyName("effort")]
    public string Effort { get; set; }

    /// <summary>
    /// Максимальное количество токенов для процесса рассуждений.
    /// Взаимоисключающее с <see cref="Effort"/>.
    /// </summary>
    [JsonPropertyName("max_tokens")]
    public int? MaxTokens { get; set; }

    /// <summary>
    /// Исключить ли токены рассуждений из ответа (по умолчанию false).
    /// При true возвращается только финальный результат без промежуточных шагов.
    /// </summary>
    [JsonPropertyName("exclude")]
    public bool Exclude { get; set; }

    /// <summary>
    /// Включены ли рассуждения. 
    /// Автоматически определяется из <see cref="Effort"/> или <see cref="MaxTokens"/>, если не указано явно.
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    #endregion

    #region Конструкторы

    /// <summary>
    /// Инициализирует новый экземпляр для сериализации.
    /// </summary>
    public ReasoningSettings() { }

    /// <summary>
    /// Инициализирует настройки рассуждений с указанными параметрами.
    /// </summary>
    /// <param name="effort">Уровень усилий: "high", "medium" или "low". Используйте константы <see cref="EffortHigh"/>, <see cref="EffortMedium"/>, <see cref="EffortLow"/>.</param>
    /// <param name="maxTokens">Максимальное количество токенов для рассуждений.</param>
    /// <param name="exclude">Исключить промежуточные шаги рассуждений из ответа.</param>
    /// <param name="enabled">Явно включить/выключить рассуждения.</param>
    /// <exception cref="ArgumentException">Выбрасывается, если указаны одновременно <paramref name="effort"/> и <paramref name="maxTokens"/>.</exception>
    public ReasoningSettings(
        string effort = null,
        int? maxTokens = null,
        bool exclude = false,
        bool enabled = false)
    {
        ValidateParameters(effort, maxTokens);

        Effort = effort;
        MaxTokens = maxTokens;
        Exclude = exclude;
        Enabled = enabled || IsReasoningConfigured(effort, maxTokens);
    }

    #endregion

    #region Вспомогательные методы

    /// <summary>
    /// Проверяет корректность параметров (взаимоисключение Effort и MaxTokens).
    /// </summary>
    private static void ValidateParameters(string effort, int? maxTokens)
    {
        if (!string.IsNullOrEmpty(effort) && maxTokens.HasValue)
        {
            throw new ArgumentException(
                "Параметры Effort и MaxTokens взаимоисключающие. Укажите только один из них.",
                nameof(effort));
        }

        if (maxTokens.HasValue && maxTokens.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxTokens),
                $"MaxTokens должен быть положительным. Получено: {maxTokens.Value}");
        }
    }

    /// <summary>
    /// Определяет, настроены ли рассуждения на основе Effort или MaxTokens.
    /// </summary>
    private static bool IsReasoningConfigured(string effort, int? maxTokens)
    {
        return !string.IsNullOrEmpty(effort) || maxTokens.HasValue;
    }

    /// <summary>
    /// Создает настройки с высоким уровнем усилий.
    /// </summary>
    public static ReasoningSettings CreateHighEffort(bool excludeSteps = false)
    {
        return new ReasoningSettings(effort: EffortHigh, exclude: excludeSteps);
    }

    /// <summary>
    /// Создает настройки с ограничением по токенам.
    /// </summary>
    public static ReasoningSettings CreateWithTokenLimit(int maxTokens, bool excludeSteps = false)
    {
        return new ReasoningSettings(maxTokens: maxTokens, exclude: excludeSteps);
    }

    #endregion
}