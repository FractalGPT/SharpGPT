using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Core.Models.Common.Messages.Content;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(TextContentItem), "text")]
[JsonDerivedType(typeof(ImageContent), "image_url")]
public interface IContentItem
{
    [JsonPropertyName("type")]
    string Type { get; }
}
