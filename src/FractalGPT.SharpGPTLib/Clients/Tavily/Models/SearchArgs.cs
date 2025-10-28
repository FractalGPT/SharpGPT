using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.Tavily.Models;

public class SearchArgs
{
    [JsonPropertyName("query")]
    public string Query { get; set; }

    [JsonPropertyName("include_answer")]
    public bool IncludeAnswer { get; set; }

    [JsonPropertyName("include_image_descriptions")]
    public bool IncludeImageDescriptions { get; set; }

    [JsonPropertyName("include_images")]
    public bool IncludeImages { get; set; }

    [JsonPropertyName("api_key")]
    public string ApiKey { get; set; }

    [JsonPropertyName("search_depth")]
    public string SearchDepth { get; set; }

    [JsonPropertyName("topic")]
    public string Topic { get; set; }

    [JsonPropertyName("max_results")]
    public int MaxResults { get; set; }

    [JsonPropertyName("include_raw_content")]
    public bool IncludeRawContent { get; set; }

    [JsonPropertyName("time_range")]
    public string TimeRange { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("include_domains")]
    public IEnumerable<string> IncludeDomains { get; set; }

    [JsonPropertyName("exclude_domains")]
    public IEnumerable<string> ExcludeDomains { get; set; }
}