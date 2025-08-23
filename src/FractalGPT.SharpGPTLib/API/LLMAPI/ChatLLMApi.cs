using FractalGPT.SharpGPTLib.API.WebUtils;
using FractalGPT.SharpGPTLib.Prompts;
using FractalGPT.SharpGPTLib.Stream;
using System.Net.Http.Json;

namespace FractalGPT.SharpGPTLib.API.LLMAPI;

/// <summary>
/// Апи для отправки запросов на LLM по стандарту OpenAI (также поддерживается DeepSeek, VLLM, OpenRouter, Replicate и тп.)
/// </summary>
[Serializable]
public class ChatLLMApi
{
    private readonly IWebAPIClient _webApi;
    private readonly string _apiKey;
    private readonly string _prompt;
    private readonly IStreamHandler _streamSender;

    public virtual string ModelName { get; set; }
    public virtual string ApiUrl { get; set; }
    public virtual string TokenizeApiUrl { get; set; }
    public event Action<string> ProxyInfo;
    

    /// <summary>
    /// Апи для отправки запросов на LLM по стандарту OpenAI (также поддерживается DeepSeek, VLLM, OpenRouter, Replicate и тп.)
    /// </summary>
    public ChatLLMApi(string apiKey, bool useProxy, string proxyPath, string modelName, string prompt, IStreamHandler streamSender = null)
    {
        if (string.IsNullOrWhiteSpace(modelName))
            throw new ArgumentNullException(nameof(modelName), "Имя модели не может быть пустым");

        _apiKey = apiKey;
        ModelName = modelName;
        _prompt = prompt;
        _streamSender = streamSender;

        if (useProxy)
        {
            _webApi = new ProxyHTTPClient(proxyPath, apiKey);
            (_webApi as ProxyHTTPClient).OnProxyError += LLMApi_OnProxyError;
        }
        else { _webApi = new WithoutProxyClient(apiKey); }

        string defaultPrompt = prompt ?? PromptsChatGPT.ChatGPTDefaltPromptRU;
    }

    /// <summary>
    /// Определяет число токенов в запросе
    /// </summary>
    /// <param name="messages">Запрос</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> TokenizeAsync(IEnumerable<LLMMessage> messages, CancellationToken cancellationToken = default)
    {
        using var response = await _webApi.PostAsJsonAsync(TokenizeApiUrl, new
        {
            messages,
            model = ModelName,
        }, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<TokenizeResult>(cancellationToken);

        return result?.Count ?? 0;
    }

    /// <summary>
    /// Отправка сообщения без контекста (потокобезопасная версия)
    /// </summary>
    /// <param name="text">Текст запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Возвращает текст ответа</returns>
    public async Task<string> SendWithoutContextTextAsync(string text, GenerateSettings generateSettings = null, CancellationToken cancellationToken = default) =>
        (await SendWithoutContextAsync(text, generateSettings, cancellationToken)).Choices[0].Message.Content.ToString();

    /// <summary>
    /// Отправка сообщения с учетом контекста (потокобезопасная версия).
    /// </summary>
    /// <param name="context">Контекст сообщений LLM.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Возвращает текст ответа.</returns>
    public async Task<string> SendWithContextTextAsync(IEnumerable<LLMMessage> context, GenerateSettings generateSettings = null, CancellationToken cancellationToken = default) =>
        (await SendWithContextAsync(context, generateSettings, cancellationToken)).Choices[0].Message.Content.ToString();

    /// <summary>
    /// Отправка сообщения без учета контекста, с заданным началом ответа
    /// </summary>
    /// <param name="context">Контекст сообщений LLM.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Возвращает текст ответа.</returns>
    public async Task<string> SendWithoutContextWithStartReturnTextAsync(string text, string answerStart, GenerateSettings generateSettings = null, CancellationToken cancellationToken = default) =>
        (await SendWithoutContextWithStartAsync(text, answerStart, generateSettings, cancellationToken)).Choices[0].Message.Content.ToString();

    /// <summary>
    /// Отправка сообщения без учета контекста
    /// (потокобезопасная версия)
    /// </summary>
    /// <param name="text"></param>
    /// <returns>Возвращает ChatCompletionsResponse с дополнительной информацией </returns>
    public async Task<ChatCompletionsResponse> SendWithoutContextAsync(string text, GenerateSettings generateSettings = null, CancellationToken cancellationToken = default)
    {
        List<LLMMessage> context = [
            LLMMessage.CreateMessage(Roles.System, _prompt),
            LLMMessage.CreateMessage(Roles.User, text)
            ];

        return await SendWithContextAsync(context, generateSettings, cancellationToken);

    }

    /// <summary>
    /// Отправка сообщения без учета контекста
    /// (потокобезопасная версия)
    /// </summary>
    /// <param name="text">Начало ответа</param>
    /// <param name="answerStart">Начало ответа</param>
    /// <returns>Возвращает ChatCompletionsResponse с дополнительной информацией </returns>
    public async Task<ChatCompletionsResponse> SendWithoutContextWithStartAsync(string text, string answerStart, GenerateSettings generateSettings = null, CancellationToken cancellationToken = default)
    {
        List<LLMMessage> context = [
            LLMMessage.CreateMessage(Roles.System, _prompt),
            LLMMessage.CreateMessage(Roles.User, text),
            LLMMessage.CreateMessage(Roles.Assistant, answerStart)
            ];

        return await SendWithContextAsync(context, generateSettings, cancellationToken);

    }


    /// <summary>
    /// Отправка сообщения без учета контекста
    /// (потокобезопасная версия)
    /// </summary>
    /// <param name="text"></param>
    /// <returns>Возвращает ChatCompletionsResponse с дополнительной информацией </returns>
    public async Task<ChatCompletionsResponse> SendWithContextAsync(IEnumerable<LLMMessage> context, GenerateSettings generateSettings = null, CancellationToken cancellationToken = default)
    {
        generateSettings = Validate(generateSettings);

        if (context == null)
            throw new ArgumentException("Контекст не может быть null.", nameof(context));

        using var webApi = new WithoutProxyClient(_apiKey);
        var sendData = new SendDataLLM(ModelName, generateSettings);
        sendData.SetMessages(context);

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
                    var result = await _streamSender.StartAsync(streamId: generateSettings.StreamId, response: response,
                        method: generateSettings.StreamMethod);
                    //TODOS Подумать как обработать ошибки
                    if (!string.IsNullOrEmpty(result))
                        return new ChatCompletionsResponse(result); // Для общности обернуто в ChatCompletionsResponse
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

                    var textResult = chatCompletionsResponse.Choices[0].Message.Content.ToString();
                    if (!string.IsNullOrEmpty(textResult))
                        return chatCompletionsResponse;
                }

            }
            catch (Exception ex)
            {
                string text = context.Last().Content.ToString(); // Получение последнего сообщения для отображения в логах
                var content = await response.Content.ReadAsStringAsync();
                exception = new Exception(content + "\n############\n" + text.Substring(0, Math.Min(text.Length, 500)), ex);

                await Task.Delay(500);
            }
        }

        throw exception;

    }

    /// <summary>
    /// Валидация настроек генерации
    /// </summary>
    /// <param name="generateSettings">Начальные настройки</param>
    /// <returns></returns>
    public GenerateSettings Validate(GenerateSettings generateSettings) 
    {
        generateSettings ??= new();

        generateSettings.Temperature = ValidateTemperature(generateSettings.Temperature);
        generateSettings.MaxTokens = ValidateMaxTokens(generateSettings.MaxTokens);

        if (generateSettings.ReasoningSettings?.MaxTokens != null)
            generateSettings.ReasoningSettings.MaxTokens = ValidateMaxTokens(generateSettings.ReasoningSettings.MaxTokens.Value);

        return generateSettings;
    }


    /// <summary>
    /// Проверяет и нормализует значение температуры
    /// </summary>
    public static double ValidateTemperature(double temperature)
    {
        if (temperature > 1.5) return 1.5;
        if (temperature < 0.0) return 0.0;
        if (temperature > 1.0)
            Console.WriteLine($"Установлена температура > 1.0 ({temperature}), высокая вероятность галлюцинаций !!!");

        return temperature;
    }

    /// <summary>
    /// Проверяет максимальное количество токенов
    /// </summary>
    public static int ValidateMaxTokens(int maxTokens)
    {
        return Math.Max(1, maxTokens);
    }


    private void LLMApi_OnProxyError(object sender, ProxyErrorEventArgs e)
    {
        ProxyInfo($"Proxy: {e.Proxy.Address}\nError: {e.Exception}");
    }



}
