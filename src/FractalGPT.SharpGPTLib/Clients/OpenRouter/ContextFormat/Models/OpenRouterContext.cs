using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.OpenRouter.ContextFormat.Models;

/// <summary>
/// Контекст диалога в формате OR
/// </summary>
public class OpenRouterContext
{
    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("characters")]
    public Dictionary<string, Character> Characters { get; set; }

    [JsonPropertyName("messages")]
    public Dictionary<string, Message> Messages { get; set; }

    [JsonPropertyName("items")]
    public Dictionary<string, Item> Items { get; set; }

    [JsonPropertyName("artifacts")]
    public Dictionary<string, object> Artifacts { get; set; }

    [JsonPropertyName("artifactFiles")]
    public Dictionary<string, object> ArtifactFiles { get; set; }

    [JsonPropertyName("artifactVersions")]
    public Dictionary<string, object> ArtifactVersions { get; set; }

    [JsonPropertyName("artifactFileContents")]
    public Dictionary<string, object> ArtifactFileContents { get; set; }
}
