using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.Tavily.Models;

public class ExtractArgs
{
    [JsonPropertyName("api_key")]
    public string ApiKey { get; set; }

    [JsonPropertyName("urls")]
    public IEnumerable<string> Urls { get; set; }

    [JsonPropertyName("include_images")]
    public bool IncludeImages { get; set; }

    [JsonPropertyName("extract_depth")]
    public string ExtractDepth { get; set; }

    [JsonPropertyName("format")]
    public string Format { get; set; }
}
