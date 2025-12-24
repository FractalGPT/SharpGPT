using System.Net;
using FractalGPT.SharpGPTLib.Clients.Base;
using FractalGPT.SharpGPTLib.Core.Abstractions;

namespace FractalGPT.SharpGPTLib.Clients.OpenRouter
{
    /// <summary>
    /// Работа с API OpenRouter
    /// </summary>
    public class OpenRouterModelApi : ChatLLMApi
    {
        public OpenRouterModelApi(string apiKey, string modelName, IStreamHandler streamSender = null, string prompt = "", IEnumerable<WebProxy> proxies = null) 
            : base(apiKey: apiKey, modelName: modelName, prompt: prompt, streamSender: streamSender, proxies: proxies)
        {
            ApiUrl = "https://openrouter.ai/api/v1/chat/completions";
            StreamOptions = new();
        }
    }
}
