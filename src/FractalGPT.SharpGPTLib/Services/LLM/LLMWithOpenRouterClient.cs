using FractalGPT.SharpGPTLib.Clients.Base;
using FractalGPT.SharpGPTLib.Clients.OpenRouter;
using FractalGPT.SharpGPTLib.Core.Abstractions;

namespace FractalGPT.SharpGPTLib.Services.LLM;

/// <summary>
/// LLM на базе OpenRouter клиента
/// </summary>
public class LLMWithOpenRouterClient : LLMBase
{
    /// <summary>
    /// LLM на базе OpenRouter клиента
    /// </summary>
    /// <param name="settingsLLM">Настройки LLM</param>
    /// <param name="streamHandler">Обработчик потоковой передачи</param>
    public LLMWithOpenRouterClient(LLMOptions settingsLLM, IStreamHandler streamHandler = null) 
        : base(Init(settingsLLM, streamHandler), settingsLLM) { }

    // Инициализация для конструктора
    private static ChatLLMApi Init(LLMOptions openRouterSettings, IStreamHandler streamHandler)
    {
        OpenRouterModelApi client = new OpenRouterModelApi(
            apiKey: openRouterSettings.ApiKey,
            modelName: openRouterSettings.ModelName,
            streamSender: streamHandler,
            prompt: openRouterSettings.SystemPrompt
            );

        return client;
    }
}
