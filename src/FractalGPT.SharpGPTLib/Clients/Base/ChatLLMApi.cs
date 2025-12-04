using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FractalGPT.SharpGPTLib.API.LLMAPI;
using FractalGPT.SharpGPTLib.Core.Abstractions;
using FractalGPT.SharpGPTLib.Core.Models.Common.Messages;
using FractalGPT.SharpGPTLib.Core.Models.Common.Requests;
using FractalGPT.SharpGPTLib.Core.Models.Common.Responses;
using FractalGPT.SharpGPTLib.Infrastructure.Extensions;
using FractalGPT.SharpGPTLib.Infrastructure.Http;
using FractalGPT.SharpGPTLib.Services.Prompts;
using Newtonsoft.Json;
using Serilog;

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

    /// <summary>
    /// Настройки для мониторинга таймаута простоя между чанками данных (по умолчанию 30 секунд)
    /// </summary>
    public IdleTimeoutSettings IdleTimeoutSettings { get; set; } = IdleTimeoutSettings.Default;

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
        else 
        { 
            _webApi = new WithoutProxyClient(apiKey);
        }

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
    /// ВНУТРИ ВСЕГДА ИСПОЛЬЗУЕТ STREAMING для раннего обнаружения зависших запросов!
    /// </summary>
    /// <param name="context">Контекст сообщений</param>
    /// <param name="generateSettings">Настройки генерации</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Возвращает ChatCompletionsResponse с дополнительной информацией </returns>
    public async Task<ChatCompletionsResponse> SendWithContextAsync(
    IEnumerable<LLMMessage> context,
    GenerateSettings generateSettings = null,
    CancellationToken cancellationToken = default)
    {
        generateSettings = Validate(generateSettings);

        if (context == null)
            throw new ArgumentException("Контекст не может быть null.", nameof(context));

        // ВАЖНО: Принудительно включаем streaming для раннего обнаружения зависших запросов!
        // Даже если пользователь не указал streamId, мы создаем временный для внутреннего использования
        if (string.IsNullOrEmpty(generateSettings.StreamId))
        {
            // Создаем временный streamId для включения streaming
            generateSettings = new GenerateSettings(
                temperature: generateSettings.Temperature ?? 0.1,
                repetitionPenalty: generateSettings.RepetitionPenalty,
                topP: generateSettings.TopP,
                topK: generateSettings.TopK,
                minTokens: generateSettings.MinTokens,
                maxTokens: generateSettings.MaxTokens,
                streamId: Guid.NewGuid().ToString(), // ← Включаем streaming!
                reasoningEffort: generateSettings.ReasoningEffort,
                streamMethod: "StreamMessage"
            )
            {
                // Копируем дополнительные свойства через инициализатор
                ReasoningSettings = generateSettings.ReasoningSettings,
                LogProbs = generateSettings.LogProbs,
                TopLogprobs = generateSettings.TopLogprobs,
            };
        }

        var sendData = new SendDataLLM(ModelName, generateSettings);
        sendData.StreamOptions = StreamOptions;
        sendData.SetMessages(context);

        const int maxAttempts = 2;
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
                // if (generateSettings.Stream)
                //    return await ProcessStreamResponse(generateSettings, response);
                // else
                //    return await ProcessStandardResponse(response, cancellationToken);

                // ВСЕГДА обрабатываем как streaming (т.к. мы принудительно его включили)
                // Но используем внутренний метод, не требующий IStreamHandler
                return await ProcessStreamResponseInternal(response, cancellationToken);
            }
            catch (TimeoutException timeoutEx)
            {
                var sendDataRaw = JsonConvert.SerializeObject(sendData).TruncateForLogging();
                Log.Error(timeoutEx, $"ChatLLMApi SendWithContext TimeoutException, ApiUrl={ApiUrl}, ModelName={ModelName}, SendData={sendDataRaw}");
                throw;
            }
            catch (TaskCanceledException taskCancelledEx)
            {
                var sendDataRaw = JsonConvert.SerializeObject(sendData).TruncateForLogging();
                Log.Error(taskCancelledEx, $"ChatLLMApi SendWithContext TaskCanceledException, ApiUrl={ApiUrl}, ModelName={ModelName}, SendData={sendDataRaw}");
                throw;
            }
            catch (OperationCanceledException taskCancelledEx)
            {
                var sendDataRaw = JsonConvert.SerializeObject(sendData).TruncateForLogging();
                Log.Error(taskCancelledEx, $"ChatLLMApi SendWithContext OperationCanceledException, ApiUrl={ApiUrl}, ModelName={ModelName}, SendData={sendDataRaw}");
                throw;
            }
            catch (Exception ex)
            {
                var sendDataRaw = JsonConvert.SerializeObject(sendData).TruncateForLogging();
                Log.Error(ex, $"ChatLLMApi SendWithContext Exception, ApiUrl={ApiUrl}, ModelName={ModelName}, SendData={sendDataRaw}");

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

        var content = (await response.Content.ReadAsStringAsync() ?? "").TruncateForLogging();

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
        string sendDataJson = JsonConvert.SerializeObject(sendData);
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
    /// Обрабатывает потоковый ответ ВНУТРЕННЕ (без IStreamHandler).
    /// Используется для автоматического streaming в SendWithContextAsync.
    /// Читает SSE stream, накапливает токены и возвращает полный ответ.
    /// </summary>
    private async Task<ChatCompletionsResponse> ProcessStreamResponseInternal(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        Log.Debug($"ChatLLMApi ProcessStreamResponseInternal: Начинаем читать stream, StatusCode={response.StatusCode}");

        Stream stream = null;
        try
        {
            using var baseStream = await response.Content.ReadAsStreamAsync();
            
            // Оборачиваем в idle timeout monitor если включено
            if (IdleTimeoutSettings != null && IdleTimeoutSettings.Enabled)
            {
                Log.Debug($"ChatLLMApi ProcessStreamResponseInternal: Включаем мониторинг idle timeout ({IdleTimeoutSettings.IdleTimeout.TotalSeconds} сек)");
                stream = new StreamWithTimeoutMonitor(baseStream, IdleTimeoutSettings.IdleTimeout, cancellationToken);
            }
            else
            {
                Log.Debug($"ChatLLMApi ProcessStreamResponseInternal: Idle timeout ОТКЛЮЧЕН или не настроен");
                stream = baseStream;
            }

            using var reader = new StreamReader(stream);
            var fullContent = new StringBuilder();
            
            // Поддержка Vision моделей - собираем изображения
            var collectedImages = new List<ImageInfo>();
            string nativeFinishReason = null;
            string finishReason = null;
            
            // Сохраняем usage из чанка (обычно последний чанк с usage != null)
            Usage collectedUsage = null;
            
            // Защита от зацикленного ответа (один и тот же токен повторяется слишком много раз)
            const int maxConsecutiveRepeats = 200;
            string lastToken = null;
            int consecutiveCount = 0;
            
            Log.Debug($"ChatLLMApi ProcessStreamResponseInternal: Stream получен, начинаем читать строки...");
            
            int linesRead = 0;
            while (!reader.EndOfStream)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException(cancellationToken);

                var line = await reader.ReadLineAsync();
                linesRead++;
                
                // Пропускаем пустые строки (с защитной задержкой от busy-loop)
                if (string.IsNullOrEmpty(line))
                {
                    await Task.Delay(10, cancellationToken);
                    continue;
                }
                
                // Пропускаем SSE комментарии (начинаются с :)
                if (line.StartsWith(":"))
                    continue;
                    
                // Маркер завершения - дочитываем оставшиеся данные
                if (line == "data: [DONE]")
                {
                    // Дочитываем оставшиеся данные (могут быть usage, метаданные)
                    while (!reader.EndOfStream)
                    {
                        var remainingLine = await reader.ReadLineAsync();
                        if (!string.IsNullOrEmpty(remainingLine))
                        {
                            Log.Debug($"ChatLLMApi: После [DONE] получена строка: {remainingLine}");
                        }
                    }
                    break;
                }

                // Обрабатываем SSE строки с данными
                if (!line.StartsWith("data: "))
                    continue;

                string jsonData = line.Substring(6); // Убираем "data: "
                
                try
                {
                    // Используем JsonDocument для низкоуровневого парсинга
                    using var parsedJson = JsonDocument.Parse(jsonData);
                    var root = parsedJson.RootElement;

                    // Проверка на null/undefined
                    if (root.ValueKind == JsonValueKind.Null || root.ValueKind == JsonValueKind.Undefined)
                        continue;

                    // Получаем choices
                    if (!root.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
                        continue;

                    var firstChoice = choices[0];
                    
                    // Получаем finish_reason и native_finish_reason (могут быть в любом чанке, но обычно в последнем)
                    if (firstChoice.TryGetProperty("finish_reason", out var finishReasonElement) && 
                        finishReasonElement.ValueKind == JsonValueKind.String)
                    {
                        finishReason = finishReasonElement.GetString();
                    }
                    
                    if (firstChoice.TryGetProperty("native_finish_reason", out var nativeFinishElement) && 
                        nativeFinishElement.ValueKind == JsonValueKind.String)
                    {
                        nativeFinishReason = nativeFinishElement.GetString();
                    }
                    
                    // Парсим usage если есть (обычно в последнем чанке)
                    if (root.TryGetProperty("usage", out var usageElement) && 
                        usageElement.ValueKind == JsonValueKind.Object)
                    {
                        collectedUsage = ParseUsageFromJson(usageElement);
                        Log.Debug($"ChatLLMApi: Получен usage - prompt_tokens={collectedUsage.PromptTokens}, " +
                                  $"completion_tokens={collectedUsage.CompletionTokens}, " +
                                  $"total_tokens={collectedUsage.TotalTokens}, " +
                                  $"cost={collectedUsage.Cost}");
                    }
                    
                    // Получаем delta
                    if (!firstChoice.TryGetProperty("delta", out var delta))
                        continue;
                    
                    // Парсим текстовый контент (delta.content)
                    if (delta.TryGetProperty("content", out var contentElement))
                    {
                        string content = contentElement.GetString() ?? string.Empty;
                        if (!string.IsNullOrEmpty(content))
                        {
                            // Проверяем на зацикленный ответ (один и тот же токен 200+ раз подряд)
                            if (content == lastToken)
                            {
                                consecutiveCount++;
                                if (consecutiveCount >= maxConsecutiveRepeats)
                                {
                                    throw new InvalidOperationException(
                                        $"Обнаружен зацикленный ответ: токен \"{lastToken}\" повторяется {consecutiveCount} раз подряд. " +
                                        "Возможно модель зависла или генерирует некорректный вывод.");
                                }
                            }
                            else
                            {
                                lastToken = content;
                                consecutiveCount = 1;
                            }
                            
                            fullContent.Append(content);
                        }
                    }
                    
                    // Парсим изображения (delta.images) - для Vision моделей
                    // ВАЖНО: Изображения сохраняем ТОЛЬКО если в этом же чанке native_finish_reason == "STOP"
                    if (delta.TryGetProperty("images", out var imagesElement) && 
                        imagesElement.ValueKind == JsonValueKind.Array)
                    {
                        // Проверяем что это финальный чанк с native_finish_reason == "STOP"
                        bool isStopChunk = string.Equals(nativeFinishReason, "STOP", StringComparison.OrdinalIgnoreCase) ||
                                          string.Equals(finishReason, "stop", StringComparison.OrdinalIgnoreCase);
                        
                        if (!isStopChunk)
                        {
                            Log.Debug($"ChatLLMApi: Получены изображения, но native_finish_reason != STOP " +
                                       $"(finish_reason={finishReason}, native_finish_reason={nativeFinishReason}). Пропускаем.");
                            continue;
                        }
                        
                        foreach (var imageElement in imagesElement.EnumerateArray())
                        {
                            var imageInfo = new ImageInfo();
                            
                            if (imageElement.TryGetProperty("type", out var typeEl))
                                imageInfo.Type = typeEl.GetString();
                            
                            if (imageElement.TryGetProperty("index", out var indexEl))
                                imageInfo.Index = indexEl.GetInt32();
                            
                            if (imageElement.TryGetProperty("image_url", out var imageUrlEl))
                            {
                                imageInfo.ImageUrl = new ImageUrl();
                                if (imageUrlEl.TryGetProperty("url", out var urlEl))
                                    imageInfo.ImageUrl.Url = urlEl.GetString();
                            }
                            
                            if (imageInfo.ImageUrl?.Url != null)
                            {
                                collectedImages.Add(imageInfo);
                                Log.Debug($"ChatLLMApi: Получено изображение, index={imageInfo.Index}, type={imageInfo.Type}");
                            }
                        }
                    }
                }
                catch (System.Text.Json.JsonException ex)
                {
                    // Пропускаем невалидные JSON чанки
                    Log.Warning(ex, $"ChatLLMApi ProcessStreamResponseInternal: невалидный JSON chunk");
                    continue;
                }
            }
            
            Log.Debug($"ChatLLMApi ProcessStreamResponseInternal: Закончили читать stream, всего строк: {linesRead}, " +
                      $"длина контента: {fullContent.Length}, изображений: {collectedImages.Count}, " +
                      $"finish_reason: {finishReason}, native_finish_reason: {nativeFinishReason}");
            
            // Дочитываем любые оставшиеся данные после выхода из цикла
            string finalLine;
            while ((finalLine = await reader.ReadLineAsync()) != null)
            {
                if (!string.IsNullOrEmpty(finalLine))
                {
                    Log.Debug($"ChatLLMApi: После завершения цикла получена строка: {finalLine}");
                }
            }

            if (!string.IsNullOrEmpty(nativeFinishReason) &&
                (string.Equals(nativeFinishReason, "IMAGE_PROHIBITED_CONTENT", StringComparison.OrdinalIgnoreCase) ||
                nativeFinishReason.Contains("PROHIBITED_CONTENT")))
            {
                return new ChatCompletionsResponse(
                    $$"""
                    К сожалению, не могу выполнить этот запрос, так как он нарушает политику использования.
                    
                    Пожалуйста, попробуйте:
                    • Переформулировать запрос
                    • Изменить изображение
                    • Убедиться, что контент соответствует правилам безопасности
                    
                    Причина: {{nativeFinishReason}}
                    """);
            }

            // Проверяем что генерация завершилась корректно
            // Разрешены: native_finish_reason = "STOP"/"MAX_TOKENS"/"length" ИЛИ finish_reason = "stop"
            if (!string.Equals(nativeFinishReason, "STOP", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(nativeFinishReason, "MAX_TOKENS", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(nativeFinishReason, "length", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(finishReason, "stop", StringComparison.OrdinalIgnoreCase))
            {
                var lastLine = await reader.ReadLineAsync();

                throw new InvalidOperationException(
                    $$"""
                    Генерация не завершилась корректно.
                    native_finish_reason='{{nativeFinishReason}}', finish_reason='{{finishReason}}'.
                    Ожидалось native_finish_reason='STOP' или 'MAX_TOKENS' или 'length', либо finish_reason='stop'.
                    Last Line: {{lastLine}}
                    """);
            }
            
            // Проверяем что есть хоть какой-то результат (текст ИЛИ изображения)
            // Примечание: изображения сохраняются только при native_finish_reason == "STOP" (фильтрация выше)
            bool hasText = fullContent.Length > 0;
            bool hasImages = collectedImages.Count > 0;
            
            if (!hasText && !hasImages)
            {
                throw new InvalidOperationException("Потоковый ответ пуст - не получено ни текста, ни изображений.");
            }

            // Формируем ответ
            var resultMessage = new LLMMessage("assistant", hasText ? fullContent.ToString() : string.Empty);
            
            // Добавляем изображения если есть
            if (hasImages)
            {
                resultMessage.Images = collectedImages;
            }
            
            return new ChatCompletionsResponse
            {
                Choices =
                [
                    new Choice
                    {
                        Message = resultMessage,
                        FinishReason = finishReason ?? "stop",
                        NativeFinishReason = nativeFinishReason
                    }
                ],
                Model = ModelName,
                Usage = collectedUsage,
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"ChatLLMApi ProcessStreamResponseInternal Exception");
            throw;
        }
        finally
        {
            stream?.Dispose();
        }
    }

    /// <summary>
    /// Обрабатывает стандартный ответ
    /// </summary>
    private async Task<ChatCompletionsResponse> ProcessStandardResponse(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        try
        {
            var chatCompletionsResponse = await response.Content
                .ReadFromJsonAsync<ChatCompletionsResponse>(cancellationToken: cancellationToken);

            if (chatCompletionsResponse == null ||
                chatCompletionsResponse.Choices == null ||
                chatCompletionsResponse.Choices.Count == 0)
            {
                var content = (await response.Content.ReadAsStringAsync() ?? "").TruncateForLogging();
                throw new InvalidOperationException($"Некорректный ответ от LLM API.\nContent={content}");
            }

            return chatCompletionsResponse;
        }
        catch (Exception ex)
        {
            string content = "";
            try { content = await response.Content.ReadAsStringAsync(); } catch { }
            Log.Error(ex, $"ChatLLMApi ProcessStandardResponse, Content={content.TruncateForLogging()}");
            throw;
        }
    }

    /// <summary>
    /// Парсит объект usage из JSON элемента
    /// </summary>
    private static Usage ParseUsageFromJson(JsonElement usageElement)
    {
        var usage = new Usage();
        
        if (usageElement.TryGetProperty("prompt_tokens", out var promptTokens))
            usage.PromptTokens = promptTokens.GetInt32();
        
        if (usageElement.TryGetProperty("completion_tokens", out var completionTokens))
            usage.CompletionTokens = completionTokens.GetInt32();
        
        if (usageElement.TryGetProperty("total_tokens", out var totalTokens))
            usage.TotalTokens = totalTokens.GetInt32();
        
        // Парсим cost с использованием готовой утилиты
        if (usageElement.TryGetProperty("cost", out var costElement) && costElement.ValueKind != JsonValueKind.Null)
            usage.Cost = costElement.Clone();
        
        // Парсим reasoning_tokens из completion_tokens_details
        if (usageElement.TryGetProperty("completion_tokens_details", out var completionDetails) && 
            completionDetails.ValueKind == JsonValueKind.Object)
        {
            if (completionDetails.TryGetProperty("reasoning_tokens", out var reasoningTokens))
                usage.ReasoningTokens = reasoningTokens.GetInt32();
        }
        
        return usage;
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
