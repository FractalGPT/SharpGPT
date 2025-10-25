using FractalGPT.SharpGPTLib.API.LLMAPI;
using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Core.Models.Common.Messages.Content;

[Serializable]
public class ImageContent : IContentItem
{
    [JsonIgnore]
    public string Type => "image_url";

    [JsonPropertyName("image_url")]
    public ImageUrl ImageUrl { get; set; }


    public ImageContent() { }

    public ImageContent(string imageUrl)
    {
        ImageUrl = new ImageUrl { Url = imageUrl };
    }

    public ImageContent(IEnumerable<byte> image)
    {
        string base64 = Convert.ToBase64String(image.ToArray());
        ImageUrl = new ImageUrl { Url = $"data:image/jpeg;base64,{base64}" };
    }
}
