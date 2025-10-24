using FractalGPT.SharpGPTLib.API.LLMAPI;
using FractalGPT.SharpGPTLib.Stream;

namespace FractalGPT.SharpGPTLib.API.OpenRouter
{
    /// <summary>
    /// Работа с API OpenRouter
    /// </summary>
    public class OpenRouterModelApi : ChatLLMApi
    {
        public OpenRouterModelApi(string apiKey, string modelName, IStreamHandler streamSender = null, string prompt = "", bool useProxy = false, string proxyPath = null) : base(apiKey: apiKey, useProxy: useProxy, proxyPath: proxyPath, modelName: modelName, prompt: prompt, streamSender: streamSender)
        {
            ApiUrl = "https://openrouter.ai/api/v1/chat/completions";
            StreamOptions = new();
        }
    }
}
