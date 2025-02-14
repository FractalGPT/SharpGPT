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
    /// <returns></returns>
    public async Task<string> SendToLLM(string text)
    {
        string answer = await _chatLLMApi.SendWithoutContextTextAsync(text);
        return answer;
    }

    /// <summary>
    /// Отправка запроса к LLM
    /// </summary>
    /// <param name="context">Несколько сообщений</param>
    /// <returns></returns>
    public async Task<string> SendToLLM(IEnumerable<LLMMessage> context)
    {
        string answer = await _chatLLMApi.SendWithContextTextAsync(context);
        return answer;
    }
}
