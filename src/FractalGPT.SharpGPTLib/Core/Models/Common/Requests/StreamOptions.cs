using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Core.Models.Common.Requests;

public class StreamOptions
{
    [JsonPropertyName("include_usage")]
    public bool IncludeUsage { get; set; } = true;
}
