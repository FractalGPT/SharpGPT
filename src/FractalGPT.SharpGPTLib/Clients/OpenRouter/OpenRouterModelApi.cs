using System.Net;
using FractalGPT.SharpGPTLib.Clients.Base;
using FractalGPT.SharpGPTLib.Core.Abstractions;
using FractalGPT.SharpGPTLib.Core.Models.Common.Messages;

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

        /// <summary>
        /// Отключаем валидацию контекста для OpenRouter (огромный контекст у Gemma 3 27B)
        /// Возвращаем 1 чтобы проверка tokensCount < MaxLLMTokens всегда проходила
        /// </summary>
        public override async Task<int> TokenizeAsync(IEnumerable<LLMMessage> messages, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(1);
        }
    }
}
