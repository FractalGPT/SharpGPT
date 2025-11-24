using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using FractalGPT.SharpGPTLib.Clients.Tavily.Models;
using FractalGPT.SharpGPTLib.Infrastructure.Extensions;

namespace FractalGPT.SharpGPTLib.Clients.Tavily;

public class TavilyClient
{
    public const string Host = "https://api.tavily.com";

    private readonly HttpClient _httpClient;

    private readonly string _apiKey;

    public TavilyClient(string apiKey, WebProxy proxy = null)
    {
        _apiKey = apiKey;
        if (string.IsNullOrEmpty(apiKey))
            throw new ArgumentException($"{nameof(apiKey)} is missing");

        var handler = new HttpClientHandler();
        if (proxy != null)
        {
            handler.UseProxy = true;
            handler.Proxy = proxy;
        }
        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(Host),
            Timeout = TimeSpan.FromMinutes(5),
        };
    }

    public async Task<SearchResult> SearchAsync(string query, int maxResults = 5, bool includeRawContent = true, bool includeAnswer = false, bool includeImages = false,
        bool includeImageDescriptions = false, SearchDepth searchDepth = SearchDepth.Basic, TopicType topic = TopicType.General, TimeRange timeRange = TimeRange.All,
        CountryType country = CountryType.All, IEnumerable<Uri> includeDomains = null, IEnumerable<Uri> excludeDomains = null)
    {
        includeDomains ??= [];
        excludeDomains ??= [];
        if (includeDomains.Count() > 300)
            throw new ArgumentException("Maximum 300 domains for includeDomains");
        if (excludeDomains.Count() > 150)
            throw new ArgumentException("Maximum 150 domains for excludeDomains");


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
        });
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<SearchResult>();

        result.Results = result.Results
            .Where(result => !ContainsForbiddenContent(url: result.Url, rawContent: result.RawContent, excludeDomains: excludeDomains))
            .ToArray();

        return result;
    }

    public async Task<ExtractResult> ExtractAsync(IEnumerable<string> urls, bool includeImages = false, ExtractDepth extractDepth = ExtractDepth.Basic, FormatType format = FormatType.Markdown)
    {
        ExtractResult result = null;
        const int maxAttempts = 3;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync("/extract", new ExtractArgs
                {
                    ApiKey = _apiKey,
                    Urls = urls,
                    IncludeImages = includeImages,
                    ExtractDepth = extractDepth.GetDescription(),
                    Format = format.GetDescription(),
                });
                response.EnsureSuccessStatusCode();
                result = await response.Content.ReadFromJsonAsync<ExtractResult>();
                if (!result?.FailedResults?.Any() ?? false) 
                    return result;
            }
            catch (Exception ex) { }

            if (attempt != maxAttempts - 1)
            await Task.Delay(TimeSpan.FromSeconds(2 * (attempt+1))); // 2, 4
        }


        return result;
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
}
