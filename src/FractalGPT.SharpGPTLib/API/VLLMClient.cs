using System;

namespace FractalGPT.SharpGPTLib.API;

/// <summary>
/// Represents a VLLM client that communicates with a language model 
/// via HTTP or HTTPS to generate chat completions.
/// </summary>
[Serializable]
public class VLLMClient : ChatLLMApi
{
    /// <summary>
    /// Specifies the protocol type to use for requests.
    /// </summary>
    public enum HTTPType
    {
        /// <summary>
        /// Hypertext Transfer Protocol.
        /// </summary>
        HTTP,

        /// <summary>
        /// Hypertext Transfer Protocol Secure.
        /// </summary>
        HTTPS
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VLLMClient"/> class.
    /// </summary>
    /// <param name="modelName">The name of the language model to use.</param>
    /// <param name="systemPrompt">The system prompt that defines the initial context or instructions.</param>
    /// <param name="temperature">Controls the randomness or creativity of the generated text.</param>
    /// <param name="ip">The IP address of the VLLM server.</param>
    /// <param name="port">The port the VLLM server is listening on.</param>
    /// <param name="httpType">The protocol type to use (HTTP or HTTPS).</param>
    /// <param name="apiKey">An optional API key for authentication, if required.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="modelName"/>, <paramref name="ip"/>, or <paramref name="port"/> is null or empty.
    /// </exception>
    public VLLMClient(
        string modelName,
        string systemPrompt,
        double temperature,
        string ip,
        string port,
        HTTPType httpType = HTTPType.HTTP,
        string apiKey = null)
        : base(apiKey, false, string.Empty, modelName, systemPrompt, temperature)
    {
        if (string.IsNullOrWhiteSpace(modelName))
            throw new ArgumentNullException(nameof(modelName), "Model name cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(ip))
            throw new ArgumentNullException(nameof(ip), "IP address cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(port))
            throw new ArgumentNullException(nameof(port), "Port cannot be null or empty.");

        var protocol = httpType == HTTPType.HTTP ? "http" : "https";
        ApiUrl = $"{protocol}://{ip}:{port}/v1/chat/completions";
    }
}