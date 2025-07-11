using FractalGPT.SharpGPTLib.API;
using FractalGPT.SharpGPTLib.API.LLMAPI;
using FractalGPT.SharpGPTLib.Stream;

namespace FractalGPT.SharpGPTLib.LLM;

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
            vLLMSettings.Temperature,
            vLLMSettings.Host,
            vLLMSettings.ApiKey,
            streamHandler
            );

        return client;
    }
}
