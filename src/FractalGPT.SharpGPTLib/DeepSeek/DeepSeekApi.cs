using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using FractalGPT.SharpGPTLib.API.LLMAPI;
using FractalGPT.SharpGPTLib.Stream;

namespace FractalGPT.SharpGPTLib.DeepSeek;

/// <summary>
/// Api для работы с DeepSeek
/// </summary>
[Serializable]
public class DeepSeekApi : ChatLLMApi
{
    public DeepSeekApi(string apiKey, string modelName, IStreamHandler streamSender = null, string prompt = "", IEnumerable<WebProxy> proxies = null) : base(apiKey: apiKey, modelName: modelName, prompt: prompt, streamSender: streamSender, proxies: proxies)
    {
        ApiUrl = "https://api.deepseek.com/v1/chat/completions";
    }
}
