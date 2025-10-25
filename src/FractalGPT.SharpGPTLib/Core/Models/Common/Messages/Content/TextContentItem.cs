using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Core.Models.Common.Messages.Content;

/// <summary>
/// Текстовая часть контента
/// </summary>
[Serializable]
public class TextContentItem : IContentItem
{
    [JsonIgnore]
    public string Type => "text";

    [JsonPropertyName("text")]
    public string Text { get; set; }

    public TextContentItem() { }

    public TextContentItem(string text)
    {
        Text = text;
    }
}
