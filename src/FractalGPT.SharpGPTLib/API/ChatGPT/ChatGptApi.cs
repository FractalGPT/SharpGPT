﻿using FractalGPT.SharpGPTLib.API.LLMAPI;

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
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="apiKey"/> or <paramref name="modelName"/> is null or empty.</exception>
        public ChatGptApi(
            string apiKey,
            bool useProxy = false,
            string proxyPath = "proxy.json",
            string modelName = "gpt-3.5-turbo",
            string prompt = null,
            double temperature = 0.7)
            : base(apiKey, useProxy, proxyPath, modelName, prompt, temperature)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException(nameof(apiKey), "API key cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(modelName))
                throw new ArgumentNullException(nameof(modelName), "Model name cannot be null or empty.");
        }
    }
}