using System.Net;
using FractalGPT.SharpGPTLib.Clients.Base;
using FractalGPT.SharpGPTLib.Core.Abstractions;

namespace FractalGPT.SharpGPTLib.Clients.DeepSeek
{
    /// <summary>
    /// Работа с API DeepSeek
    /// </summary>
    public class DeepSeekModelApi : ChatLLMApi
    {
        public DeepSeekModelApi(string apiKey, string modelName, IStreamHandler streamSender = null, string prompt = "", IEnumerable<WebProxy> proxies = null) 
            : base(apiKey: apiKey, modelName: modelName, prompt: prompt, streamSender: streamSender, proxies: proxies)
        {
            ApiUrl = "https://api.deepseek.com/chat/completions";
            StreamOptions = new();
        }
    }
}

