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
    /// Установка системного промпта
    /// </summary>
    /// <param name="prompt"></param>
    public void SetPrompt(string prompt)
    {
        _chatLLMApi.SetPrompt(prompt);
    }

    /// <summary>
    /// Отправка запроса к LLM
    /// </summary>
    /// <param name="text">Текст запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Ответ LLM в виде строки</returns>
    public async Task<string> SendToLLM(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Текст запроса не может быть пустым.", nameof(text));

        // Используем ConfigureAwait для библиотечного кода.
        return await _chatLLMApi.SendWithoutContextTextAsync(text, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Отправка запроса к LLM с учетом контекста сообщений.
    /// </summary>
    /// <param name="context">Последовательность сообщений LLM.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Ответ LLM в виде строки.</returns>
    public async Task<string> SendToLLM(IEnumerable<LLMMessage> context, CancellationToken cancellationToken = default)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        // Передаём запрос через клиент _chatLLMApi с поддержкой контекста
        return await _chatLLMApi.SendWithContextTextAsync(context, cancellationToken).ConfigureAwait(false);
    }
}
