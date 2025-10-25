using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Core.Models.Common.Responses;

public class TokenizeResult
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("max_model_len")]
    public int MaxModelLen { get; set; }
}
