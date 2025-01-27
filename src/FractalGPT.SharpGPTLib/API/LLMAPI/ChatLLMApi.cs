using FractalGPT.SharpGPTLib.API.ChatGPT;
using FractalGPT.SharpGPTLib.API.WebUtils;
using FractalGPT.SharpGPTLib.Prompts;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FractalGPT.SharpGPTLib.API;

/// <summary>
/// Class for interacting with LLM through API.
/// </summary>
[Serializable]
public class ChatLLMApi : IText2TextChat, IDisposable
{
    private readonly IWebAPIClient _webApi;
    private readonly SendDataLLM _sendData;
    public virtual string ApiUrl { get; set; }

    public event Action<string> ProxyInfo;

    /// <summary>
    /// Constructor for API initialization.
    /// </summary>
    public ChatLLMApi(string key, bool useProxy, string proxyPath, string modelName, string prompt, double t)
    {
        // Use the default prompt if a custom one is not provided.
        if (useProxy)
        {
            _webApi = new ProxyHTTPClient(proxyPath, key);
            (_webApi as ProxyHTTPClient).OnProxyError += LLMApi_OnProxyError;
        }
        else { _webApi = new WithoutProxyClient(key); }

        string defaultPrompt = prompt ?? PromptsChatGPT.ChatGPTDefaltPromptRU;
        _sendData = new SendDataLLM(modelName, defaultPrompt, temp: t);


    }

    private void LLMApi_OnProxyError(object sender, ProxyErrorEventArgs e)
    {
        ProxyInfo($"Proxy: {e.Proxy.Address}\nError: {e.Exception}");
    }

    /// <summary>
    /// Asynchronous method for sending text and receiving a response from LLM, maintaining the context of the dialogue.
    /// </summary>
    public async Task<ChatCompletionsResponse> SendAsync(string text)
    {
        // Adding user text to the messages.
        _sendData.AddUserMessage(text);

        // Sending the request and receiving the response.
        HttpResponseMessage response = await _webApi.PostAsJsonAsync(ApiUrl, _sendData);

        // Deserialize the response into a ChatCompletionsResponse object.
        ChatCompletionsResponse chatCompletionsResponse = await response.Content.ReadFromJsonAsync<ChatCompletionsResponse>();

        // Add the system's response to the messages to maintain context.
        _sendData.AddAssistantMessage(chatCompletionsResponse.Choices[0].Message.Content);

        return chatCompletionsResponse;
    }

    /// <summary>
    /// Asynchronous method for sending text and returning only the text content of the response from LLM.
    /// </summary>
    public async Task<string> SendAsyncReturnText(string text)
    {
        var chatCompletionsResponse = await SendAsync(text);
        return chatCompletionsResponse.Choices[0].Message.Content;
    }

    /// <summary>
    /// Asynchronous method for sending text
    /// </summary>
    public async void SendAsyncText(string text)
    {
        var chatCompletionsResponse = await SendAsync(text);
        Answer(chatCompletionsResponse.Choices[0].Message.Content);
    }

    /// <summary>
    /// Asynchronous method for sending context of the dialogue and receiving a response from LLM
    /// </summary>
    /// <param name="roleMessages">Role - message. Dictionary: "role" -> bot or user, "text" -> massage</param>
    public async Task<ChatCompletionsResponse> SendAsync(IEnumerable<Dictionary<string, string>> roleMessages)
    {
        // Set context
        SetContext(roleMessages);
        // Sending the request and receiving the response.
        HttpResponseMessage response = await _webApi.PostAsJsonAsync(ApiUrl, _sendData);
        // Deserialize the response into a ChatCompletionsResponse object.
        ChatCompletionsResponse chatCompletionsResponse = await response.Content.ReadFromJsonAsync<ChatCompletionsResponse>();

        return chatCompletionsResponse;
    }


    /// <summary>
    /// Asynchronous method for sending context
    /// </summary>
    /// <param name="roleMessages">Role - message. Dictionary: "role" -> bot or user, "text" -> massage</param>
    public async void SendContext(IEnumerable<Dictionary<string, string>> roleMessages)
    {
        var chatCompletionsResponse = await SendAsync(roleMessages);
        Answer(chatCompletionsResponse.Choices[0].Message.Content);
    }

    /// <summary>
    /// Synchronous method for sending text and receiving a response from LLM, maintaining the context of the dialogue.
    /// </summary>
    public ChatCompletionsResponse Send(string text)
    {
        // Adding user text to the messages.
        _sendData.AddUserMessage(text);

        // Sending the request and receiving the response.
        HttpResponseMessage response = _webApi.PostAsJsonAsync(ApiUrl, _sendData).Result;

        // Check for successful response status, otherwise an exception will be thrown.
        _ = response.EnsureSuccessStatusCode();

        // Deserialize the response into a ChatCompletionsResponse object.
        ChatCompletionsResponse chatCompletionsResponse = response.Content.ReadFromJsonAsync<ChatCompletionsResponse>().Result;

        // Add the system's response to the messages to maintain context.
        _sendData.AddAssistantMessage(chatCompletionsResponse.Choices[0].Message.Content);

        return chatCompletionsResponse;
    }

    /// <summary>
    /// Synchronous method for sending text and returning only the text content of the response from LLM.
    /// </summary>
    public string SendReturnText(string text)
    {
        var chatCompletionsResponse = Send(text);
        return chatCompletionsResponse.Choices[0].Message.Content;
    }

    /// <summary>
    /// Set context
    /// </summary>
    /// <param name="roleMessages"></param>
    public void SetContext(IEnumerable<Dictionary<string, string>> roleMessages)
    {
        _sendData.Clear();

        foreach (var roleMess in roleMessages)
        {
            if (roleMess["role"] == "bot")
                _sendData.AddAssistantMessage(roleMess["text"]);
            else _sendData.AddUserMessage(roleMess["text"]);
        }
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

    public void Dispose()
    {

    }

    public event Action<string> Answer;
}
