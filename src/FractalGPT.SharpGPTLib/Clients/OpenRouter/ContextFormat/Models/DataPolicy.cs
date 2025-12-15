using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.OpenRouter.ContextFormat.Models;

// Политика данных
public class DataPolicy
{
    [JsonPropertyName("training")]
    public bool Training { get; set; }

    [JsonPropertyName("trainingOpenRouter")]
    public bool TrainingOpenRouter { get; set; }

    [JsonPropertyName("retainsPrompts")]
    public bool RetainsPrompts { get; set; }

    [JsonPropertyName("canPublish")]
    public bool CanPublish { get; set; }

    [JsonPropertyName("termsOfServiceURL")]
    public string TermsOfServiceUrl { get; set; }

    [JsonPropertyName("privacyPolicyURL")]
    public string PrivacyPolicyUrl { get; set; }
}
