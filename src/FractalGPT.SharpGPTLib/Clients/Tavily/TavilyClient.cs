using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading;
using FractalGPT.SharpGPTLib.Clients.Tavily.Models;
using FractalGPT.SharpGPTLib.Infrastructure.Extensions;
using Serilog;

namespace FractalGPT.SharpGPTLib.Clients.Tavily;

public class TavilyClient : IDisposable
{
    public const string Host = "https://api.tavily.com";

    private readonly HttpClient _httpClient;
    private readonly HttpClientHandler _httpHandler;
    private readonly string _apiKey;
    private int _disposed; // 0 = not disposed, 1 = disposed (для Interlocked)

    public TavilyClient(string apiKey, WebProxy proxy = null)
    {
        // Trim обязателен для обоих мест использования ключа:
        // - тело запроса: Tavily не триммит api_key сам, "key\n" дает 401,
        //   при этом api_key из тела имеет приоритет над заголовком (проверено live);
        // - заголовок: .NET бросает исключение на перенос строки в значении заголовка.
        apiKey = apiKey?.Trim();

        _apiKey = apiKey;
        if (string.IsNullOrEmpty(apiKey))
            throw new ArgumentException($"{nameof(apiKey)} is missing");

        _httpHandler = new HttpClientHandler();
        if (proxy != null)
        {
            _httpHandler.UseProxy = true;
            _httpHandler.Proxy = proxy;
        }
        _httpClient = new HttpClient(_httpHandler)
        {
            BaseAddress = new Uri(Host),
            Timeout = TimeSpan.FromSeconds(60),
        };

        // Актуальный способ аутентификации Tavily - заголовок Authorization: Bearer <key>.
        // Поле api_key в теле запроса осталось для обратной совместимости и в документации больше не значится,
        // поэтому передаем ключ обоими способами (ключ уже триммлен выше).
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async Task<SearchResult> SearchAsync(string query, int maxResults = 5, bool includeRawContent = true, bool includeAnswer = false, bool includeImages = false,
        bool includeImageDescriptions = false, SearchDepth searchDepth = SearchDepth.Basic, TopicType topic = TopicType.General, TimeRange timeRange = TimeRange.All,
        CountryType country = CountryType.All, IEnumerable<Uri> includeDomains = null, IEnumerable<Uri> excludeDomains = null, CancellationToken cancellationToken = default)
    {
        includeDomains ??= [];
        excludeDomains ??= [];
        if (includeDomains.Count() > 300)
            throw new ArgumentException("Maximum 300 domains for includeDomains");
        if (excludeDomains.Count() > 150)
            throw new ArgumentException("Maximum 150 domains for excludeDomains");

        const int maxAttempts = 2;
        Exception lastException = null;
        
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            try
            {
                // Локальный таймаут 60 секунд для ReadFromJsonAsync
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

                using var response = await _httpClient.PostAsJsonAsync("/search", new SearchArgs
                {
                    ApiKey = _apiKey,
                    IncludeAnswer = includeAnswer,
                    IncludeImages = includeImages,
                    IncludeImageDescriptions = includeImageDescriptions,
                    Query = query,
                    MaxResults = maxResults,
                    SearchDepth = searchDepth.GetDescription(),
                    IncludeRawContent = includeRawContent,
                    Topic = topic.GetDescription(),
                    TimeRange = timeRange.GetDescription(),
                    Country = country.GetDescription(),
                    IncludeDomains = includeDomains.Select(domain => domain.AbsoluteUri),
                    ExcludeDomains = excludeDomains.Select(domain => domain.AbsoluteUri),
                }, cancellationToken);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<SearchResult>(cancellationToken: linkedCts.Token);

                // Ответ внешнего API может оказаться пустым - не роняем вызывающий код NullReferenceException
                if (result == null)
                    throw new HttpRequestException("Tavily search вернул пустой ответ");

                result.Results = (result.Results ?? [])
                    .Where(result => !ContainsForbiddenContent(url: result.Url, rawContent: result.RawContent, excludeDomains: excludeDomains))
                    .ToArray();

                return result;
            }
            catch (Exception) when (cancellationToken.IsCancellationRequested)
            {
                throw; // Глобальная отмена - не делаем retry
            }
            catch (Exception ex)
            {
                lastException = ex;
                Log.Warning(ex, "TavilyClient SearchAsync attempt {Attempt}/{MaxAttempts} failed, Query={Query}",
                    attempt + 1, maxAttempts, query);

                if (attempt < maxAttempts - 1) // Только для первой попытки
                {
                    try { await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken); }
                    catch (OperationCanceledException) { throw lastException; }
                }
            }
        }

        throw lastException ?? new Exception("Tavily search failed after 2 attempts");
    }

    public async Task<ExtractResult> ExtractAsync(IEnumerable<string> urls, bool includeImages = false, ExtractDepth extractDepth = ExtractDepth.Basic, FormatType format = FormatType.Markdown, CancellationToken cancellationToken = default)
    {
        ExtractResult result = null;
        Exception lastException = null;
        const int maxAttempts = 4;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                // Локальный таймаут 60 секунд для ReadFromJsonAsync
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

                using var response = await _httpClient.PostAsJsonAsync("/extract", new ExtractArgs
                {
                    ApiKey = _apiKey,
                    Urls = urls,
                    IncludeImages = includeImages,
                    ExtractDepth = extractDepth.GetDescription(),
                    Format = format.GetDescription(),
                }, cancellationToken);
                response.EnsureSuccessStatusCode();
                result = await response.Content.ReadFromJsonAsync<ExtractResult>(cancellationToken: linkedCts.Token);

                // ВАЖНО: успех - это наличие извлеченного контента.
                // Раньше здесь было `!result?.FailedResults?.Any() ?? false`, что из-за приоритета операторов
                // давало null ?? false == false для успешного ответа без секции failed_results,
                // поэтому даже удачный extract всегда прогонялся 4 раза с задержками 2+4+6 секунд.
                if (result?.Results?.Any() ?? false)
                    return result;
            }
            catch (Exception) when (cancellationToken.IsCancellationRequested)
            {
                throw; // Глобальная отмена - не делаем retry
            }
            catch (Exception ex)
            {
                lastException = ex;
                Log.Warning(ex, "TavilyClient ExtractAsync attempt {Attempt}/{MaxAttempts} failed, Urls={Urls}",
                    attempt + 1, maxAttempts, string.Join(", ", urls));
            }

            if (attempt != maxAttempts - 1)
                await Task.Delay(TimeSpan.FromSeconds(2 * (attempt+1)), cancellationToken); // 2, 4, 6
        }

        // Никогда не возвращаем null: вызывающий код не должен падать с NullReferenceException.
        // Причину неудачи отдаем через FailedResults, чтобы она дошла до агента.
        return result ?? new ExtractResult
        {
            Results = [],
            FailedResults = urls.Select(url => new ExtractItemFailedResult
            {
                Url = url,
                Error = lastException?.Message ?? $"Tavily extract не вернул результат за {maxAttempts} попытки",
            }).ToArray(),
        };
    }

    /// <summary>
    /// Фильтрация результата поиска на наличие недопустимой/запрещенной/устаревшей/нерелевантной информации
    /// </summary>
    /// <param name="url">Адрес источника</param>
    /// <param name="rawContent">Контент источника</param>
    /// <param name="excludeDomains">Запрещенные домены</param>
    /// <returns>Возвращает true если результат содержит запрещенную информацию и false если допустимую информацию</returns>
    public virtual bool ContainsForbiddenContent(string url, string rawContent, IEnumerable<Uri> excludeDomains)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        if (string.IsNullOrEmpty(rawContent))
            return true;

        var uri = new Uri(url);
        if (excludeDomains != null && excludeDomains.Any() && excludeDomains.Any(excludeDomain => excludeDomain.Host == uri.Host))
            return true;

        if (Regex.IsMatch(url, @"\b(?:ua|\.ua)\b", RegexOptions.CultureInvariant))
            return true;

        return false;
    }

    public void Dispose()
    {
        Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (Interlocked.CompareExchange(ref _disposed, 1, 0) != 0)
            return;
        
        if (disposing)
        {
            _httpClient?.Dispose();
            _httpHandler?.Dispose();
        }
    }
}
