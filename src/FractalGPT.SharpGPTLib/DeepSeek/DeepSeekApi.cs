using FractalGPT.SharpGPTLib.API.LLMAPI;
using FractalGPT.SharpGPTLib.Stream;
using System;
using System.Collections.Generic;
using System.Text;

namespace FractalGPT.SharpGPTLib.DeepSeek;

/// <summary>
/// Api для работы с DeepSeek
/// </summary>
[Serializable]
public class DeepSeekApi : ChatLLMApi
{
    public DeepSeekApi(string apiKey, string modelName, IStreamHandler streamSender = null, string prompt = "", bool useProxy = false, string proxyPath = null) : base(apiKey: apiKey, useProxy: useProxy, proxyPath: proxyPath, modelName: modelName, prompt: prompt, streamSender: streamSender)
    {
        ApiUrl = "https://api.deepseek.com/v1/chat/completions";
    }
}
