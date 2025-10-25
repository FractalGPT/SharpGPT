using System.Net;
using FractalGPT.SharpGPTLib.Clients.Base;
using FractalGPT.SharpGPTLib.Core.Abstractions;

namespace FractalGPT.SharpGPTLib.Clients.Perplexity;

public class PerplexityModelApi : ChatLLMApi
{
    public PerplexityModelApi(string apiKey, string modelName, IStreamHandler streamSender = null, string prompt = "", IEnumerable<WebProxy> proxies = null) : base(apiKey: apiKey, modelName: modelName, prompt: prompt, streamSender: streamSender, proxies: proxies)
    {
        ApiUrl = "https://api.perplexity.ai/chat/completions";
    }
}
