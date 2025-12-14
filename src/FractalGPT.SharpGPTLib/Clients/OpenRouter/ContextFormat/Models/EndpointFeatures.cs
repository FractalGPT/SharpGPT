using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.OpenRouter.ContextFormat.Models;

// Функции эндпоинта
public class EndpointFeatures
{
    [JsonPropertyName("supports_input_audio")]
    public bool SupportsInputAudio { get; set; }

    [JsonPropertyName("supports_tool_choice")]
    public ToolChoiceSupport SupportsToolChoice { get; set; }

    [JsonPropertyName("supported_parameters")]
    public Dictionary<string, object> SupportedParameters { get; set; }
}
