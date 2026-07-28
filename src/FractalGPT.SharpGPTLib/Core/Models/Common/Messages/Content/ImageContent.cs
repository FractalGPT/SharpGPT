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
        var bytes = image as byte[] ?? image.ToArray();
        string base64 = Convert.ToBase64String(bytes);
        ImageUrl = new ImageUrl { Url = $"data:{DetectMimeType(bytes)};base64,{base64}" };
    }

    /// <summary>
    /// Определяет MIME-тип изображения по сигнатуре файла.
    /// Раньше любые байты помечались как image/jpeg, из-за чего PNG/WEBP уходили
    /// в провайдер с неверным типом и генерация падала с finish_reason='error'.
    /// </summary>
    private static string DetectMimeType(byte[] bytes)
    {
        const string defaultMimeType = "image/png";

        if (bytes == null || bytes.Length < 12)
            return defaultMimeType;

        // PNG: 89 50 4E 47 0D 0A 1A 0A
        if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
            return "image/png";

        // GIF: "GIF8"
        if (bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x38)
            return "image/gif";

        // BMP: "BM"
        if (bytes[0] == 0x42 && bytes[1] == 0x4D)
            return "image/bmp";

        // WEBP: "RIFF"...."WEBP"
        if (bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46 &&
            bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50)
            return "image/webp";

        // JPEG: FF D8 FF
        if (bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            return "image/jpeg";

        return defaultMimeType;
    }
}
