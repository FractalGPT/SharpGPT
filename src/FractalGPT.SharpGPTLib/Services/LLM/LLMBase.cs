using FractalGPT.SharpGPTLib.Clients.Base;
using FractalGPT.SharpGPTLib.Core.Models.Common.Messages;
using FractalGPT.SharpGPTLib.Core.Models.Common.Requests;

namespace FractalGPT.SharpGPTLib.Services.LLM;

/// <summary>
/// Базовая логика работы с LLM
/// </summary>
public class LLMBase
{
    private readonly ChatLLMApi _chatLLMApi;
    protected readonly LLMOptions _llmOptions;

    /// <summary>
    /// Базовая логика работы с LLM
    /// </summary>
    /// <param name="chatLLMApi">Общий класс для работы с LLM</param>
    /// <param name="llmOptions">Настройки LLM (опционально)</param>
    public LLMBase(ChatLLMApi chatLLMApi, LLMOptions llmOptions = null)
    {
        _chatLLMApi = chatLLMApi;
        _llmOptions = llmOptions;
    }

    /// <summary>
    /// Создает GenerateSettings с учетом reasoning параметров из LLMOptions
    /// </summary>
    /// <param name="baseSettings">Базовые настройки генерации</param>
    /// <returns>GenerateSettings с примененными reasoning параметрами</returns>
    protected GenerateSettings ApplyReasoningSettings(GenerateSettings baseSettings)
    {
        if (_llmOptions == null || !_llmOptions.EnableReasoning)
            return baseSettings ?? new GenerateSettings();

        var settings = baseSettings ?? new GenerateSettings();
        
        // Создаем ReasoningSettings если reasoning включен
        if (_llmOptions.EnableReasoning)
        {
            settings.ReasoningSettings = new ReasoningSettings(
                effort: _llmOptions.ReasoningEffort,
                maxTokens: _llmOptions.ReasoningMaxTokens,
                exclude: _llmOptions.ReasoningExclude,
                enabled: true
            );
        }

        return settings;
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

        // Применяем reasoning настройки
        generateSettings = ApplyReasoningSettings(generateSettings);

        // Используем ConfigureAwait для библиотечного кода.
        return await _chatLLMApi.SendWithoutContextTextAsync(text, generateSettings, cancellationToken).ConfigureAwait(false);
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

        // Применяем reasoning настройки
        generateSettings = ApplyReasoningSettings(generateSettings);

        // Передаём запрос через клиент _chatLLMApi с поддержкой контекста
        return await _chatLLMApi.SendWithContextTextAsync(messages, generateSettings, cancellationToken).ConfigureAwait(false);
    }

    public async Task<int> TokenizeAsync(IEnumerable<LLMMessage> messages, CancellationToken cancellationToken = default)
    {
        if (messages == null)
            throw new ArgumentNullException(nameof(messages));

        return await _chatLLMApi.TokenizeAsync(messages, cancellationToken).ConfigureAwait(false);
    }
}
