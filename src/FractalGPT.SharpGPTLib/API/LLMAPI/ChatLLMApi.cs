using FractalGPT.SharpGPTLib.API.WebUtils;
using FractalGPT.SharpGPTLib.Prompts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FractalGPT.SharpGPTLib.API.LLMAPI;

/// <summary>
/// Class for interacting with LLM through API.
/// </summary>
[Serializable]
public class ChatLLMApi : IText2TextChat, IDisposable
{
    private readonly IWebAPIClient _webApi;
    private readonly SendDataLLM _sendData;

    private readonly string _modelName;
    private readonly string _apiKey;
    private readonly string _prompt;
    private readonly double _temperature;

    public virtual string ApiUrl { get; set; }

    public event Action<string> ProxyInfo;

    /// <summary>
    /// Constructor for API initialization.
    /// </summary>
    public ChatLLMApi(string key, bool useProxy, string proxyPath, string modelName, string prompt, double temperature)
    {
        _apiKey = key;
        _modelName = modelName;
        _prompt = prompt;
        _temperature = temperature;

        // Use the default prompt if a custom one is not provided.
        if (useProxy)
        {
            _webApi = new ProxyHTTPClient(proxyPath, key);
            (_webApi as ProxyHTTPClient).OnProxyError += LLMApi_OnProxyError;
        }
        else { _webApi = new WithoutProxyClient(key); }

        string defaultPrompt = prompt ?? PromptsChatGPT.ChatGPTDefaltPromptRU;
        _sendData = new SendDataLLM(modelName, defaultPrompt, temp: temperature);


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
        _sendData.AddUserMessage(text);
        HttpResponseMessage response = await _webApi.PostAsJsonAsync(ApiUrl, _sendData);
        ChatCompletionsResponse chatCompletionsResponse = await response.Content.ReadFromJsonAsync<ChatCompletionsResponse>();
        _sendData.AddAssistantMessage(chatCompletionsResponse.Choices[0].Message.Content);

        return chatCompletionsResponse;
    }

    /// <summary>
    /// Asynchronous method for sending text and returning only the text content of the response from LLM.
    /// </summary>
    public async Task<string> SendReturnTextAsync(string text)
    {
        var chatCompletionsResponse = await SendAsync(text);
        return chatCompletionsResponse.Choices[0].Message.Content;
    }


    /// <summary>
    /// Отправка сообщения без контекста 
    /// (потокобезопасная версия)
    /// </summary>
    /// <param name="text"></param>
    /// <returns>Возвращает текст ответа</returns>
    public async Task<string> SendWithoutContextTextAsync(string text)
    {
        var webApi = new WithoutProxyClient(_apiKey);
        var sendData = new SendDataLLM(_modelName, _prompt, temp: _temperature);

        sendData.AddUserMessage(text);
        HttpResponseMessage response = await webApi.PostAsJsonAsync(ApiUrl, sendData);
        ChatCompletionsResponse chatCompletionsResponse = await response.Content
            .ReadFromJsonAsync<ChatCompletionsResponse>();
        return chatCompletionsResponse.Choices[0].Message.Content;
    }

    /// <summary>
    /// Отправка сообщения учитывающая контекст 
    /// (потокобезопасная версия)
    /// </summary>
    /// <param name="context"></param>
    /// <returns>Возвращает текст ответа</returns>
    public async Task<string> SendWithContextTextAsync(IEnumerable<LLMMessage> context)
    {
        var webApi = new WithoutProxyClient(_apiKey);
        var sendData = new SendDataLLM(_modelName, _prompt, temp: _temperature);
        sendData.SetMessages(context);

        HttpResponseMessage response = await webApi.PostAsJsonAsync(ApiUrl, sendData);
        ChatCompletionsResponse chatCompletionsResponse = await response.Content
            .ReadFromJsonAsync<ChatCompletionsResponse>();
        return chatCompletionsResponse.Choices[0].Message.Content;
    }

    /// <summary>
    /// Отправка сообщения без учета контекста
    /// (потокобезопасная версия)
    /// </summary>
    /// <param name="text"></param>
    /// <returns>Возвращает ChatCompletionsResponse с дополнительной информацией </returns>
    public async Task<ChatCompletionsResponse> SendWithoutContextAsync(string text)
    {
        var webApi = new WithoutProxyClient(_apiKey);
        var sendData = new SendDataLLM(_modelName, _prompt, temp: _temperature);

        sendData.AddUserMessage(text);
        HttpResponseMessage response = await webApi.PostAsJsonAsync(ApiUrl, sendData);
        ChatCompletionsResponse chatCompletionsResponse = await response.Content
            .ReadFromJsonAsync<ChatCompletionsResponse>();

        return chatCompletionsResponse;
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
        SetContext(roleMessages);
        HttpResponseMessage response = await _webApi.PostAsJsonAsync(ApiUrl, _sendData);
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
        _sendData.AddUserMessage(text);
        HttpResponseMessage response = _webApi.PostAsJsonAsync(ApiUrl, _sendData).Result;
        _ = response.EnsureSuccessStatusCode();
        ChatCompletionsResponse chatCompletionsResponse = response.Content.ReadFromJsonAsync<ChatCompletionsResponse>().Result;
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
    /// Sets a new system prompt and resets the context
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
