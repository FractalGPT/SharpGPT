using FractalGPT.SharpGPTLib.API.LLMAPI;
using FractalGPT.SharpGPTLib.Stream;
using System;
using System.Collections.Generic;
using System.Text;

namespace FractalGPT.SharpGPTLib.API.OpenRouter
{
    /// <summary>
    /// Работа с API OpenRouter
    /// </summary>
    public class OpenRouterModelApi : ChatLLMApi
    {
        public OpenRouterModelApi(string key, string modelName, IStreamHandler streamSender = null, string prompt = "", bool useProxy = false, string proxyPath = null) : base(key, useProxy, proxyPath, modelName, prompt, streamSender)
        {
            ApiUrl = "https://openrouter.ai/api/v1/chat/completions";
        }
    }
}
