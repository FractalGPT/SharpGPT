using FractalGPT.SharpGPTLib.API.LLMAPI;

namespace FractalGPT.SharpGPTLib.API;

/// <summary>
/// Represents a VLLM client that communicates with a language model 
/// via HTTP or HTTPS to generate chat completions.
/// </summary>
[Serializable]
public class VLLMClient : ChatLLMApi
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VLLMClient"/> class.
    /// </summary>
    /// <param name="modelName">The name of the language model to use.</param>
    /// <param name="systemPrompt">The system prompt that defines the initial context or instructions.</param>
    /// <param name="temperature">Controls the randomness or creativity of the generated text.</param>
    /// <param name="host">The host the VLLM server is listening on</param>
    /// <param name="apiKey">An optional API key for authentication, if required.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="modelName"/>, <paramref name="ip"/>, or <paramref name="port"/> is null or empty.
    /// </exception>
    public VLLMClient(
        string modelName,
        string systemPrompt,
        double temperature,
        string host,
        string apiKey = null)
        : base(apiKey, false, string.Empty, modelName, systemPrompt, temperature)
    {
        if (string.IsNullOrWhiteSpace(modelName))
            throw new ArgumentNullException(nameof(modelName), "Model name cannot be null or empty.");

        ApiUrl = $"{host.Trim('/')}/v1/chat/completions";
        TokenizeApiUrl = $"{host.Trim('/')}/tokenize";
    }
}