using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.OpenRouter.ContextFormat.Models;

// Провайдер
public class ProviderInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; }

    [JsonPropertyName("slug")]
    public string Slug { get; set; }

    [JsonPropertyName("baseUrl")]
    public string BaseUrl { get; set; }

    [JsonPropertyName("dataPolicy")]
    public DataPolicy DataPolicy { get; set; }

    [JsonPropertyName("headquarters")]
    public string Headquarters { get; set; }

    [JsonPropertyName("regionOverrides")]
    public Dictionary<string, RegionOverride> RegionOverrides { get; set; }

    [JsonPropertyName("hasChatCompletions")]
    public bool HasChatCompletions { get; set; }

    [JsonPropertyName("hasCompletions")]
    public bool HasCompletions { get; set; }

    [JsonPropertyName("isAbortable")]
    public bool IsAbortable { get; set; }

    [JsonPropertyName("moderationRequired")]
    public bool ModerationRequired { get; set; }

    [JsonPropertyName("editors")]
    public List<string> Editors { get; set; }

    [JsonPropertyName("owners")]
    public List<string> Owners { get; set; }

    [JsonPropertyName("adapterName")]
    public string AdapterName { get; set; }

    [JsonPropertyName("isMultipartSupported")]
    public bool IsMultipartSupported { get; set; }

    [JsonPropertyName("statusPageUrl")]
    public string StatusPageUrl { get; set; }

    [JsonPropertyName("byokEnabled")]
    public bool ByokEnabled { get; set; }

    [JsonPropertyName("icon")]
    public Icon Icon { get; set; }

    [JsonPropertyName("ignoredProviderModels")]
    public List<string> IgnoredProviderModels { get; set; }

    [JsonPropertyName("sendClientIp")]
    public bool SendClientIp { get; set; }
}
