using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FractalGPT.SharpGPTLib.Prompts;

namespace FractalGPT.SharpGPTLib.API.ChatGPT;

/// <summary>
/// Class for interacting with ChatGPT through API.
/// </summary>
[Serializable]
public class ChatGptApi : IText2TextAPI
{
    private readonly HttpClient _httpClient;
    private readonly SendDataChatGPT _sendData;
    private static readonly string ApiUrl = "https://api.openai.com/v1/chat/completions";

    /// <summary>
    /// Constructor for API initialization.
    /// </summary>
    public ChatGptApi(string key, string modelName = "gpt-3.5-turbo", string prompt = null, double t = 0.7)
    {
        // Use the default prompt if a custom one is not provided.
        string defaultPrompt = prompt ?? PromptsChatGPT.ChatGPTDefaltPromptRU;

        _sendData = new SendDataChatGPT(modelName, defaultPrompt, temp: t);

        _httpClient = new HttpClient
        {
            DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Bearer", key) }
        };
    }

    /// <summary>
    /// Asynchronous method for sending text and receiving a response from ChatGPT, maintaining the context of the dialogue.
    /// </summary>
    public async Task<ChatCompletionsResponse> SendAsync(string text)
    {
        // Adding user text to the messages.
        _sendData.AddUserMessage(text);

        // Sending the request and receiving the response.
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(ApiUrl, _sendData);

        // Check for successful response status, otherwise an exception will be thrown.
        response.EnsureSuccessStatusCode();

        // Deserialize the response into a ChatCompletionsResponse object.
        ChatCompletionsResponse chatCompletionsResponse = await response.Content.ReadFromJsonAsync<ChatCompletionsResponse>();

        // Add the system's response to the messages to maintain context.
        _sendData.AddAssistantMessage(chatCompletionsResponse.Choices[0].Message.Content);

        return chatCompletionsResponse;
    }

    /// <summary>
    /// Asynchronous method for sending text and returning only the text content of the response from ChatGPT.
    /// </summary>
    public async Task<string> SendAsyncReturnText(string text)
    {
        var chatCompletionsResponse = await SendAsync(text);
        return chatCompletionsResponse.Choices[0].Message.Content;
    }

    /// <summary>
    /// Synchronous method for sending text and receiving a response from ChatGPT, maintaining the context of the dialogue.
    /// </summary>
    public ChatCompletionsResponse Send(string text)
    {
        // Adding user text to the messages.
        _sendData.AddUserMessage(text);

        // Sending the request and receiving the response.
        HttpResponseMessage response = _httpClient.PostAsJsonAsync(ApiUrl, _sendData).Result;

        // Check for successful response status, otherwise an exception will be thrown.
        response.EnsureSuccessStatusCode();

        // Deserialize the response into a ChatCompletionsResponse object.
        ChatCompletionsResponse chatCompletionsResponse = response.Content.ReadFromJsonAsync<ChatCompletionsResponse>().Result;

        // Add the system's response to the messages to maintain context.
        _sendData.AddAssistantMessage(chatCompletionsResponse.Choices[0].Message.Content);

        return chatCompletionsResponse;
    }

    /// <summary>
    /// Synchronous method for sending text and returning only the text content of the response from ChatGPT.
    /// </summary>
    public string SendReturnText(string text)
    {
        var chatCompletionsResponse = Send(text);
        return chatCompletionsResponse.Choices[0].Message.Content;
    }

    /// <summary>
    /// Clears the current dialogue context, preserving the initial system prompt.
    /// </summary>
    public void ClearContext() => _sendData.Clear();

    /// <summary>
    /// Sets a new system prompt and resets the context.
    /// </summary>
    public void SetPrompt(string prompt)
    {
        _sendData.Prompt = prompt;
        _sendData.Clear();
    }
}
