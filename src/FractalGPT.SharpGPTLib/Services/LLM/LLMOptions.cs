using FractalGPT.SharpGPTLib.Core.Models.Common.Requests;

namespace FractalGPT.SharpGPTLib.Services.LLM;

/// <summary>
/// Настройки для LLM клиента
/// </summary>
public class LLMOptions
{
    /// <summary>
    /// Имя модели (vLLM)
    /// </summary>
    public string ModelName { get; set; }

    /// <summary>
    /// Базовая системная подсказка для модели
    /// </summary>
    public string SystemPrompt { get; set; }

    /// <summary>
    /// Значение температуры для LLM 
    /// (степень в которую возводятся вероятности активации T)
    /// </summary>
    public double Temperature { get; set; }

    /// <summary>
    /// Апи-ключ для подключения к модели (если необходим)
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// Расположение vllm сервера
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// Включить ли режим reasoning (для мыслительных моделей)
    /// </summary>
    public bool EnableReasoning { get; set; }

    /// <summary>
    /// Максимальное количество токенов для процесса рассуждений
    /// </summary>
    public int? ReasoningMaxTokens { get; set; }

    /// <summary>
    /// Уровень усилий для рассуждений: "high", "medium" или "low"
    /// </summary>
    public string ReasoningEffort { get; set; }

    /// <summary>
    /// Исключить ли токены рассуждений из ответа (по умолчанию false)
    /// </summary>
    public bool ReasoningExclude { get; set; }

    /// <summary>
    /// Предпочтительный провайдер OpenRouter (используется если ProviderType = OpenRouter).
    /// Если null — OpenRouter сам выбирает провайдера.
    /// </summary>
    public ProviderPreference PreferredProvider { get; set; }
}
