using FractalGPT.SharpGPTLib.API.LLMAPI;
using FractalGPT.SharpGPTLib.Core.Abstractions;
using FractalGPT.SharpGPTLib.Core.Models.Common.Messages;
using FractalGPT.SharpGPTLib.Core.Models.Common.Requests;
using FractalGPT.SharpGPTLib.Core.Models.Common.Responses;
using FractalGPT.SharpGPTLib.Infrastructure.Http;
using FractalGPT.SharpGPTLib.Services.Prompts;
using Serilog;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FractalGPT.SharpGPTLib.Clients.Base;

/// <summary>
/// Апи для отправки запросов на LLM по стандарту OpenAI (также поддерживается DeepSeek, VLLM, OpenRouter, Replicate и тп.)
/// </summary>
[Serializable]
public class ChatLLMApi
{
    private readonly IWebAPIClient _webApi;
    private readonly string _prompt;
    private readonly IStreamHandler _streamSender;

    public virtual string ModelName { get; set; }
    public virtual string ApiUrl { get; set; }
    public virtual string TokenizeApiUrl { get; set; }

    public StreamOptions StreamOptions { get; set; }

    public event Action<string> ProxyInfo;


    /// <summary>
    /// Апи для отправки запросов на LLM по стандарту OpenAI (также поддерживается DeepSeek, VLLM, OpenRouter, Replicate и тп.)
    /// </summary>
    public ChatLLMApi(string apiKey, string modelName, string prompt, IStreamHandler streamSender = null,
        IEnumerable<WebProxy> proxies = null)
    {
        if (string.IsNullOrWhiteSpace(modelName))
            throw new ArgumentNullException(nameof(modelName), "Имя модели не может быть пустым");

        ModelName = modelName;
        _prompt = prompt;
        _streamSender = streamSender;
        // Возможно стоит заменить на логер
        ProxyInfo += ChatLLMApi_ProxyInfo;

        if (proxies != null && proxies.Any())
        {
            _webApi = new ProxyHTTPClient(proxies, apiKey);
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
        SendDataLLM sendData = new SendDataLLM(ModelName);
        sendData.SetMessages(messages);

        using var response = await _webApi.PostAsJsonAsync(TokenizeApiUrl, sendData, cancellationToken);
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
    public async Task<ChatCompletionsResponse> SendWithContextAsync(
    IEnumerable<LLMMessage> context,
    GenerateSettings generateSettings = null,
    CancellationToken cancellationToken = default)
    {
        generateSettings = Validate(generateSettings);

        if (context == null)
            throw new ArgumentException("Контекст не может быть null.", nameof(context));

        var sendData = new SendDataLLM(ModelName, generateSettings);
        sendData.StreamOptions = StreamOptions;
        sendData.SetMessages(context);

        const int maxAttempts = 3;
        const int initialDelaySeconds = 1;
        Exception lastException = new Exception("Базовая ошибка");

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                using var response = await _webApi.PostAsJsonAsync(ApiUrl, sendData, cancellationToken);

                // Проверка успешности HTTP-запроса
                if (!response.IsSuccessStatusCode)
                {
                    lastException = await CreateHttpErrorException(
                        attempt,
                        response,
                        context,
                        cancellationToken);

                    // Задержка перед следующей попыткой
                    if (attempt < maxAttempts - 1)
                        await DelayWithExponentialBackoff(attempt, initialDelaySeconds, cancellationToken);

                    continue;
                }

                // Обработка успешного ответа
                if (generateSettings.Stream)
                    return await ProcessStreamResponse(generateSettings, response);
                else
                    return await ProcessStandardResponse(response, cancellationToken);
            }
            catch (TimeoutException timeoutEx)
            {
                Log.Error(timeoutEx, $"ChatLLMApi SendWithContext timeout exception, ApiUrl={ApiUrl}, ModelName={ModelName}");
                throw timeoutEx;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"ChatLLMApi SendWithContext exception, ApiUrl={ApiUrl}, ModelName={ModelName}");

                lastException = await CreateProcessingErrorException(
                    attempt,
                    ex,
                    context,
                    sendData,
                    cancellationToken);

                // Задержка для обработки исключений
                if (attempt < maxAttempts - 1)
                    await DelayWithExponentialBackoff(attempt, initialDelaySeconds, cancellationToken);
            }
        }

        throw lastException;
    }

    /// <summary>
    /// Выполняет задержку с экспоненциальным увеличением времени ожидания
    /// </summary>
    /// <param name="attempt">Номер текущей попытки (начиная с 0)</param>
    /// <param name="initialDelaySeconds">Начальная задержка в секундах</param>
    /// <param name="cancellationToken">Токен отмены</param>
    private static async Task DelayWithExponentialBackoff(
        int attempt,
        int initialDelaySeconds,
        CancellationToken cancellationToken)
    {
        // Экспоненциальная задержка: 1, 2, 4, 8, 16 секунд
        int delaySeconds = initialDelaySeconds * (int)Math.Pow(2, attempt);
        await Task.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken);
    }

    /// <summary>
    /// Создает исключение для ошибки HTTP-запроса
    /// </summary>
    private async Task<Exception> CreateHttpErrorException(
        int attempt,
        HttpResponseMessage response,
        IEnumerable<LLMMessage> context,
        CancellationToken cancellationToken)
    {
        string lastMessage = context.Last().Content.ToString();
        string truncatedMessage = lastMessage.Substring(0, Math.Min(lastMessage.Length, 512));

        var content = await response.Content.ReadAsStringAsync();
        if (!string.IsNullOrEmpty(content))
        {
            content = content.Substring(0, Math.Min(content.Length, 1024));
        }

        return new Exception(
            $"Attempt #{attempt + 1}/{5}\n" +
            $"Query: {truncatedMessage}\n" +
            $"###\n" +
            $"StatusCode: {response.StatusCode}\n" +
            $"IsCancellationRequested: {cancellationToken.IsCancellationRequested}\n" +
            $"Content: {content}\n" +
            $"###");
    }

    /// <summary>
    /// Создает исключение для ошибки обработки ответа
    /// </summary>
    private async Task<Exception> CreateProcessingErrorException(
        int attempt,
        Exception innerException,
        IEnumerable<LLMMessage> context,
        SendDataLLM sendData,
        CancellationToken cancellationToken)
    {
        string sendDataJson = JsonSerializer.Serialize(sendData);
        sendDataJson = sendDataJson.Substring(0, Math.Min(sendDataJson.Length, 512));

        string lastMessage = context.Last().Content.ToString();
        string truncatedMessage = lastMessage.Substring(0, Math.Min(lastMessage.Length, 512));

        return new Exception(
            $"Attempt #{attempt + 1}\n" +
            $"Query: {truncatedMessage}\n" +
            $"###\n" +
            $"IsCancellationRequested: {cancellationToken.IsCancellationRequested}\n" +
            $"SendData: {sendDataJson}\n" +
            $"###",
            innerException);
    }

    /// <summary>
    /// Обрабатывает потоковый ответ
    /// </summary>
    private async Task<ChatCompletionsResponse> ProcessStreamResponse(
        GenerateSettings generateSettings,
        HttpResponseMessage response)
    {
        var result = await _streamSender.StartAsync(
            streamId: generateSettings.StreamId,
            response: response,
            method: generateSettings.StreamMethod);

        // TODO: Подумать как обработать ошибки
        if (!string.IsNullOrEmpty(result))
        {
            return new ChatCompletionsResponse(result);
        }

        throw new InvalidOperationException("Потоковый ответ пуст.");
    }

    /// <summary>
    /// Обрабатывает стандартный ответ
    /// </summary>
    private async Task<ChatCompletionsResponse> ProcessStandardResponse(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var chatCompletionsResponse = await response.Content
            .ReadFromJsonAsync<ChatCompletionsResponse>(cancellationToken: cancellationToken);

        if (chatCompletionsResponse == null ||
            chatCompletionsResponse.Choices == null ||
            chatCompletionsResponse.Choices.Count == 0)
        {
            throw new InvalidOperationException("Некорректный ответ от LLM API.");
        }

        return chatCompletionsResponse;
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
    public static double? ValidateTemperature(double? temperature)
    {
        if (temperature == null) 
            return null;

        if (temperature > 1.5)
            return 1.5;
        if (temperature < 0.0)
            return 0.0;

        return temperature;
    }

    /// <summary>
    /// Проверяет максимальное количество токенов
    /// </summary>
    public static int? ValidateMaxTokens(int? maxTokens)
    {
        if (maxTokens == null) return null;

        return Math.Max(1, maxTokens.Value);
    }


    private void LLMApi_OnProxyError(object sender, ProxyErrorEventArgs e)
    {
        ProxyInfo($"Proxy: {e.Proxy.Address}\nError: {e.Exception}");
    }

    private void ChatLLMApi_ProxyInfo(string obj)
    {
        
    }



    #region Тестирование

    /// <summary>
    /// Метод предназначенный в первую очередь для тестирования 
    /// (он показывает, что отправляется в модель)
    /// </summary>
    /// <param name="text"></param>
    /// <param name="generateSettings"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public SendDataLLM GetSendDataAsync(string text, GenerateSettings generateSettings = null, CancellationToken cancellationToken = default)
    {
        List<LLMMessage> context = [
            LLMMessage.CreateMessage(Roles.System, _prompt),
            LLMMessage.CreateMessage(Roles.User, text)
            ];

        generateSettings = Validate(generateSettings);

        if (context == null)
            throw new ArgumentException("Контекст не может быть null.", nameof(context));

        var sendData = new SendDataLLM(ModelName, generateSettings);
        sendData.StreamOptions = StreamOptions;
        sendData.SetMessages(context);

        return sendData;
    }

    #endregion
}
