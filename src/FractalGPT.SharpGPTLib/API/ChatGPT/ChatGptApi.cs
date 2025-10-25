using System.Net;
using FractalGPT.SharpGPTLib.API.LLMAPI;
using FractalGPT.SharpGPTLib.Stream;

namespace FractalGPT.SharpGPTLib.API.ChatGPT
{
    /// <summary>
    /// Provides functionality for interacting with ChatGPT via OpenAI's API.
    /// </summary>
    [Serializable]
    public class ChatGptApi : ChatLLMApi
    {
        /// <summary>
        /// Gets or sets the endpoint URL for ChatGPT requests.
        /// </summary>
        public override string ApiUrl { get; set; } = "https://api.openai.com/v1/chat/completions";

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatGptApi"/> class.
        /// </summary>
        /// <param name="apiKey">The API key used to authenticate with OpenAI.</param>
        /// <param name="useProxy">Indicates whether to route requests through a proxy.</param>
        /// <param name="proxyPath">The path to the proxy configuration file.</param>
        /// <param name="modelName">The name of the model to use for generating responses.</param>
        /// <param name="prompt">An optional initial prompt to set the context of the conversation.</param>
        /// <param name="temperature">Controls the randomness or creativity of the generated output.</param>
        /// <param name="streamSender">Controls streaming behaviour of output.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="apiKey"/> or <paramref name="modelName"/> is null or empty.</exception>
        public ChatGptApi(
            string apiKey,
            string modelName = "gpt-3.5-turbo",
            string prompt = null,
            IStreamHandler streamSender = null,
            IEnumerable<WebProxy> proxies = null)
            : base(apiKey, modelName, prompt, streamSender, proxies: proxies)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException(nameof(apiKey), "API key cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(modelName))
                throw new ArgumentNullException(nameof(modelName), "Model name cannot be null or empty.");
        }
    }
}