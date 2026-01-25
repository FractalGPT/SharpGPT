using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.OpenRouter.ContextFormat.Models;

public class RegionOverride
{
    [JsonPropertyName("baseUrl")]
    public string BaseUrl { get; set; }
}
