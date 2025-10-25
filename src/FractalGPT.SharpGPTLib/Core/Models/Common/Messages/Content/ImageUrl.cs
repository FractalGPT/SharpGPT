using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Core.Models.Common.Messages.Content;

public class ImageUrl
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}
