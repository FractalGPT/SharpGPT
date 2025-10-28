using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.Tavily.Models;

public class SearchResult
{
    [JsonPropertyName("query")]
    public string Query { get; set; }

    [JsonPropertyName("response_time")]
    public double ResponseTime { get; set; }

    [JsonPropertyName("results")]
    public IEnumerable<SearchItemResult> Results { get; set; }
}

public class SearchItemResult
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("score")]
    public double Score { get; set; }

    [JsonPropertyName("raw_content")]
    public string RawContent { get; set; }
}
