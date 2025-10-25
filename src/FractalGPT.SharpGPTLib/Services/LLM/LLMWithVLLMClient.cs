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
    /// <param name="vLLNHost">Где размещена модель (адрес сервера)</param>
    public LLMWithVLLMClient(LLMOptions settingsLLM, IStreamHandler streamHandler = null) : base(Init(settingsLLM, streamHandler)) { }

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
