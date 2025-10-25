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
}
