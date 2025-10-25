using System.Net;
using FractalGPT.SharpGPTLib.API.LLMAPI;
using FractalGPT.SharpGPTLib.Stream;

namespace FractalGPT.SharpGPTLib.API.GoogleAIStudio;

public class GoogleAIStudioApi : ChatLLMApi
{
    public GoogleAIStudioApi(string apiKey, string modelName, IStreamHandler streamSender = null, string prompt = "", IEnumerable<WebProxy> proxies = null) : base(apiKey: apiKey, modelName: modelName, prompt: prompt, streamSender: streamSender, proxies: proxies)
    {
        ApiUrl = "https://generativelanguage.googleapis.com/v1beta/openai/chat/completions";
    }
}
