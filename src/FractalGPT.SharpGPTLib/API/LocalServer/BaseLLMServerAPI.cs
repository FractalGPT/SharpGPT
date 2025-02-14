using FractalGPT.SharpGPTLib.API.LocalServer.LocalServerAnswer;
using System.Net;
using System.Net.Http.Json;

namespace FractalGPT.SharpGPTLib.API.LocalServer;

/// <summary>
/// Represents a base API client for interacting with a local server that provides LLM (Large Language Models) functionalities.
/// </summary>
[Serializable]
public class BaseLLMServerAPI
{
    private readonly HttpClient _client;

    /// <summary>
    /// The base URL of the local server.
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseAPI"/> class with an optional host URL.
    /// </summary>
    /// <param name="host">The base URL of the local server.</param>
    public BaseLLMServerAPI(string host = "http://127.0.0.1:8080/")
    {
        Host = host;
        _client = new HttpClient();
    }

    /// <summary>
    /// Sets the LLM model by specifying its name and type.
    /// </summary>
    /// <param name="modelName">The name of the model.</param>
    /// <param name="modelType">The type of the model.</param>
    /// <returns>The HTTP status code indicating the result of the operation.</returns>
    public async Task<HttpStatusCode> SetLLM(string modelName, string modelType)
    {
        var uri = $"{Host}load_llm_model/";
        var modelData = new { model_name = modelName, model_type = modelType };
        var response = await _client.PostAsJsonAsync(uri, modelData);
        return response.StatusCode;
    }

    /// <summary>
    /// Generates text based on a given prompt and optional parameters.
    /// </summary>
    /// <param name="prompt">The input prompt for text generation.</param>
    /// <param name="maxLen">The maximum length of the generated text.</param>
    /// <param name="temperature">The temperature for controlling randomness.</param>
    /// <param name="topK">The number of top-k tokens considered at each step.</param>
    /// <param name="topP">The cumulative probability for top-p sampling.</param>
    /// <param name="noRepeatNgramSize">The size of n-grams that must not repeat.</param>
    /// <returns>The generated text or null if the operation failed.</returns>
    public async Task<string> TextGeneration(string prompt, int maxLen = 50, double temperature = 0.6, int topK = 15, double topP = 0.8, int noRepeatNgramSize = 3)
    {
        try
        {
            var uri = $"{Host}text_generation/";
            var requestData = new
            {
                prompt = prompt,
                max_length = maxLen,
                temperature = temperature,
                top_k = topK,
                top_p = topP,
                no_repeat_ngram_size = noRepeatNgramSize
            };
            var response = await _client.PostAsJsonAsync(uri, requestData);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<TextGenerationJSON>();
                return content?.Answer;
            }
            return null;
        }
        catch (Exception)
        {
            // ToDo: Log the exception or handle it as needed
            return null;
        }
    }


    /// <summary>
    /// Generates text based on a given prompt and optional parameters.
    /// </summary>
    /// <param name="prompt">The input prompt for text generation.</param>
    /// <param name="generationParametrs">Optional parameters to customize the generation process.</param>
    /// <returns>The generated text or null if the operation failed.</returns>
    public async Task<string> TextGeneration(string prompt, GenerationParametrs generationParametrs)
    {
        if (generationParametrs == null) generationParametrs = new GenerationParametrs();

        return await TextGeneration(prompt, generationParametrs.MaxLen, generationParametrs.Temperature,
            generationParametrs.TopK, generationParametrs.TopP, generationParametrs.NoRepeatNgramSize);
    }
}
