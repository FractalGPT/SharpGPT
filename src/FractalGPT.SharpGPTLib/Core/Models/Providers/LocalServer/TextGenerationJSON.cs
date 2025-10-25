using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Core.Models.Providers.LocalServer;

[Serializable]
public class TextGenerationJSON
{
    [JsonPropertyName("answer")]
    public string Answer { get; set; }
}
