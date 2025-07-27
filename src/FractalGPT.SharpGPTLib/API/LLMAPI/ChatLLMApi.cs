using FractalGPT.SharpGPTLib.API.WebUtils;
using FractalGPT.SharpGPTLib.Prompts;
using FractalGPT.SharpGPTLib.Stream;
using System.Net.Http.Json;
using System.Threading;

namespace FractalGPT.SharpGPTLib.API.LLMAPI;

/// <summary>
/// Class for interacting with LLM through API.
/// </summary>
[Serializable]
public class ChatLLMApi : IText2TextChat
{
    private readonly IWebAPIClient _webApi;
    private readonly SendDataLLM _sendData;

    private readonly string _modelName;
    private readonly string _apiKey;
    private readonly string _prompt;
    private readonly double _temperature;
    private readonly IStreamHandler _streamSender;

    public virtual string ApiUrl { get; set; }

    public virtual string TokenizeApiUrl { get; set; }

    public event Action<string> ProxyInfo;

    /// <summary>
    /// Constructor for API initialization.
    /// </summary>
    public ChatLLMApi(string key, bool useProxy, string proxyPath, string modelName, string prompt, double temperature, IStreamHandler streamSender)
    {
        _apiKey = key;
        _modelName = modelName;
        _prompt = prompt;
        _temperature = temperature;
        _streamSender = streamSender;

        // Use the default prompt if a custom one is not provided.
        if (useProxy)
        {
            _webApi = new ProxyHTTPClient(proxyPath, key);
            (_webApi as ProxyHTTPClient).OnProxyError += LLMApi_OnProxyError;
        }
        else { _webApi = new WithoutProxyClient(key); }

        string defaultPrompt = prompt ?? PromptsChatGPT.ChatGPTDefaltPromptRU;
        _sendData = new SendDataLLM(modelName, defaultPrompt, new GenerateSettings(temperature: temperature));
    }

    private void LLMApi_OnProxyError(object sender, ProxyErrorEventArgs e)
    {
        ProxyInfo($"Proxy: {e.Proxy.Address}\nError: {e.Exception}");
    }

    public async Task<int> TokenizeAsync(IEnumerable<LLMMessage> messages, CancellationToken cancellationToken = default)
    {
        using var response = await _webApi.PostAsJsonAsync(TokenizeApiUrl, new
        {
            messages,
            model = _modelName,
        }, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<TokenizeResult>(cancellationToken);

        return result?.Count ?? 0;
    }

    /// <summary>
    /// Asynchronous method for sending text and receiving a response from LLM, maintaining the context of the dialogue.
    /// </summary>
    public async Task<ChatCompletionsResponse> SendAsync(string text, CancellationToken cancellationToken = default)
    {
        _sendData.AddUserMessage(text);
        using HttpResponseMessage response = await _webApi.PostAsJsonAsync(ApiUrl, _sendData, cancellationToken);
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
    /// Отправка сообщения без контекста (потокобезопасная версия)
    /// </summary>
    /// <param name="text">Текст запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Возвращает текст ответа</returns>
    public async Task<string> SendWithoutContextTextAsync(string text, GenerateSettings generateSettings = null, CancellationToken cancellationToken = default)
    {
        generateSettings ??= new();

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Текст запроса не может быть пустым.", nameof(text));

        using var webApi = new WithoutProxyClient(_apiKey);
        var sendData = new SendDataLLM(_modelName, _prompt, generateSettings);
        sendData.AddUserMessage(text);

        Exception exception = new Exception();

        for (int attempts = 0; attempts < 2; attempts++)
        {
            using var response = await webApi.PostAsJsonAsync(ApiUrl, sendData, cancellationToken);

            // Проверка, что HTTP-запрос выполнен успешно
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                exception = new HttpRequestException($"Ошибка при вызове LLM API. Код статуса: {response.StatusCode}. Ответ: {errorContent}");
            }

            try
            {
                if (generateSettings.Stream)
                {
                    var result = await _streamSender.StartStreamAsync(generateSettings.StreamId, response);
                    //TODOS Подумать как обработать ошибки
                    if (!string.IsNullOrEmpty(result))
                        return result;
                }
                else
                {
                    var chatCompletionsResponse = await response.Content
                        .ReadFromJsonAsync<ChatCompletionsResponse>(cancellationToken: cancellationToken);

                    if (chatCompletionsResponse == null ||
                        chatCompletionsResponse.Choices == null ||
                        chatCompletionsResponse.Choices.Count == 0)
                    {
                        throw new InvalidOperationException("Некорректный ответ от LLM API.");
                    }

                    var result = chatCompletionsResponse.Choices[0].Message.Content;
                    if (!string.IsNullOrEmpty(result))
                        return result;
                }

            }
            catch (Exception ex)
            {
                var content = await response.Content.ReadAsStringAsync();
                exception = new Exception(content + "\n############\n" + text.Substring(0, Math.Min(text.Length, 500)), ex);

                await Task.Delay(500);
            }
        }

        throw exception;
    }

    /// <summary>
    /// Отправка сообщения с учетом контекста (потокобезопасная версия).
    /// </summary>
    /// <param name="context">Контекст сообщений LLM.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Возвращает текст ответа.</returns>
    public async Task<string> SendWithContextTextAsync(IEnumerable<LLMMessage> context, GenerateSettings generateSettings = null, CancellationToken cancellationToken = default)
    {
        generateSettings ??= new();

        if (context == null)
            throw new ArgumentNullException(nameof(context));

        using var webApi = new WithoutProxyClient(_apiKey);
        var sendData = new SendDataLLM(_modelName, _prompt, generateSettings);
        sendData.SetMessages(context);

        using var response = await webApi.PostAsJsonAsync(ApiUrl, sendData, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Ошибка при вызове LLM API. Код статуса: {response.StatusCode}. Ответ: {errorContent}");
        }

        if (generateSettings.Stream)
        {
            var result = await _streamSender.StartStreamAsync(generateSettings.StreamId, response);
            return result;
        }
        else
        {
            ChatCompletionsResponse chatCompletionsResponse = await response.Content
                .ReadFromJsonAsync<ChatCompletionsResponse>(cancellationToken: cancellationToken);

            if (chatCompletionsResponse == null ||
                chatCompletionsResponse.Choices == null ||
                chatCompletionsResponse.Choices.Count == 0)
            {
                throw new InvalidOperationException("Некорректный ответ от LLM API.");
            }

            return chatCompletionsResponse.Choices[0].Message.Content;
        }
    }

    /// <summary>
    /// Отправка сообщения без учета контекста
    /// (потокобезопасная версия)
    /// </summary>
    /// <param name="text"></param>
    /// <returns>Возвращает ChatCompletionsResponse с дополнительной информацией </returns>
    public async Task<ChatCompletionsResponse> SendWithoutContextAsync(string text, GenerateSettings generateSettings = null, CancellationToken cancellationToken = default)
    {
        generateSettings ??= new();

        var webApi = new WithoutProxyClient(_apiKey);
        var sendData = new SendDataLLM(_modelName, _prompt, generateSettings);

        sendData.AddUserMessage(text);
        using var response = await webApi.PostAsJsonAsync(ApiUrl, sendData);
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
        using HttpResponseMessage response = await _webApi.PostAsJsonAsync(ApiUrl, _sendData);
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
        using HttpResponseMessage response = _webApi.PostAsJsonAsync(ApiUrl, _sendData).Result;
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

    public event Action<string> Answer;
}
