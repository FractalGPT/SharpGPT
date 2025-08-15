using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.API.LLMAPI.MessageTypes;

public class ImageUrl
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}
