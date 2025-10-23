using FractalGPT.SharpGPTLib.API.LLMAPI;
using FractalGPT.SharpGPTLib.Stream;

namespace FractalGPT.SharpGPTLib.API.Perplexity;

public class PerplexityModelApi : ChatLLMApi
{
    public PerplexityModelApi(string apiKey, string modelName, IStreamHandler streamSender = null, string prompt = "", bool useProxy = false, string proxyPath = null) : base(apiKey: apiKey, useProxy: useProxy, proxyPath: proxyPath, modelName: modelName, prompt: prompt, streamSender: streamSender)
    {
        ApiUrl = "https://api.perplexity.ai/chat/completions";
    }
}
