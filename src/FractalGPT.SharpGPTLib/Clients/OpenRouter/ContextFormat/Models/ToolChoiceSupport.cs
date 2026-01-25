using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.OpenRouter.ContextFormat.Models;

public class ToolChoiceSupport
{
    [JsonPropertyName("literal_none")]
    public bool LiteralNone { get; set; }

    [JsonPropertyName("literal_auto")]
    public bool LiteralAuto { get; set; }

    [JsonPropertyName("literal_required")]
    public bool LiteralRequired { get; set; }

    [JsonPropertyName("type_function")]
    public bool TypeFunction { get; set; }
}
