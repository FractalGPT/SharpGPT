using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.Tavily.Models;

public class ExtractResult
{
    [JsonPropertyName("results")]
    public IEnumerable<ExtractItemResult> Results { get; set; }

    [JsonPropertyName("failed_results")]
    public IEnumerable<ExtractItemFailedResult> FailedResults { get; set; }
}

public class ExtractItemResult
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("raw_content")]
    public string RawContent { get; set; }

    // public IEnumerable<> Images { get; set; }
}

public class ExtractItemFailedResult
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("error")]
    public string Error { get; set; }
}