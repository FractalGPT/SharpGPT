using FractalGPT.SharpGPTLib.API.LLMAPI;

namespace FractalGPT.SharpGPTLib.LLM;

/// <summary>
/// Базовая логика работы с LLM
/// </summary>
public class LLMBase
{
    private readonly ChatLLMApi _chatLLMApi;

    /// <summary>
    /// Базовая логика работы с LLM
    /// </summary>
    /// <param name="chatLLMApi">Общий класс для работы с LLM</param>
    public LLMBase(ChatLLMApi chatLLMApi)
    {
        _chatLLMApi = chatLLMApi;
    }

    /// <summary>
    /// Отправка запроса к LLM
    /// </summary>
    /// <param name="text">Текст запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Ответ LLM в виде строки</returns>
    public async Task<string> SendToLLM(string text, GenerateSettings generateSettings = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Текст запроса не может быть пустым.", nameof(text));

        // Используем ConfigureAwait для библиотечного кода.
        return await _chatLLMApi.SendWithoutContextTextAsync(text, generateSettings, cancellationToken);
    }

    /// <summary>
    /// Отправка запроса к LLM с учетом контекста сообщений.
    /// </summary>
    /// <param name="messages">Последовательность сообщений LLM.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Ответ LLM в виде строки.</returns>
    public async Task<string> SendToLLM(IEnumerable<LLMMessage> messages, GenerateSettings generateSettings = null, CancellationToken cancellationToken = default)
    {
        if (messages == null)
            throw new ArgumentNullException(nameof(messages));

        // Передаём запрос через клиент _chatLLMApi с поддержкой контекста
        return await _chatLLMApi.SendWithContextTextAsync(messages, generateSettings, cancellationToken);
    }

    public async Task<int> TokenizeAsync(IEnumerable<LLMMessage> messages, CancellationToken cancellationToken = default)
    {
        if (messages == null)
            throw new ArgumentNullException(nameof(messages));

        return await _chatLLMApi.TokenizeAsync(messages, cancellationToken);
    }
}
