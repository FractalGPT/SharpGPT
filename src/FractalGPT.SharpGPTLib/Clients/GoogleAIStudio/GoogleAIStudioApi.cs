using System.Net;
using FractalGPT.SharpGPTLib.Clients.Base;
using FractalGPT.SharpGPTLib.Core.Abstractions;

namespace FractalGPT.SharpGPTLib.Clients.GoogleAIStudio;

public class GoogleAIStudioApi : ChatLLMApi
{
    public GoogleAIStudioApi(string apiKey, string modelName, IStreamHandler streamSender = null, string prompt = "", IEnumerable<WebProxy> proxies = null) : base(apiKey: apiKey, modelName: modelName, prompt: prompt, streamSender: streamSender, proxies: proxies)
    {
        ApiUrl = "https://generativelanguage.googleapis.com/v1beta/openai/chat/completions";
    }
}
