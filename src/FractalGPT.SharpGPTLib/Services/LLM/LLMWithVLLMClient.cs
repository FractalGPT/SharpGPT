using FractalGPT.SharpGPTLib.Clients.Base;
using FractalGPT.SharpGPTLib.Clients.VLLM;
using FractalGPT.SharpGPTLib.Core.Abstractions;

namespace FractalGPT.SharpGPTLib.Services.LLM;

/// <summary>
/// LLM на базе vLLM клиента
/// </summary>
public class LLMWithVLLMClient : LLMBase
{
    /// <summary>
    /// LLM на базе vLLM клиента
    /// </summary>
    /// <param name="settingsLLM">Настройки LLM</param>
    /// <param name="streamHandler">Обработчик стриминга</param>
    public LLMWithVLLMClient(LLMOptions settingsLLM, IStreamHandler streamHandler = null) 
        : base(Init(settingsLLM, streamHandler), settingsLLM) { }

    // Инициализация для конструктора
    private static ChatLLMApi Init(LLMOptions vLLMSettings, IStreamHandler streamHandler)
    {
        VLLMClient client = new VLLMClient(
            vLLMSettings.ModelName,
            vLLMSettings.SystemPrompt,
            vLLMSettings.Host,
            vLLMSettings.ApiKey,
            streamHandler
            );

        return client;
    }
}
