using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.API.LLMAPI.MessageTypes;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(TextContentItem), "text")]
[JsonDerivedType(typeof(ImageContent), "image_url")]
public interface IContentItem
{
    [JsonPropertyName("type")]
    string Type { get; }
}
