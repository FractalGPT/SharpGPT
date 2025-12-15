using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.OpenRouter.ContextFormat.Models;

// Эндпоинт
public class Endpoint
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("context_length")]
    public int ContextLength { get; set; }

    [JsonPropertyName("model")]
    public ModelInfo Model { get; set; }

    [JsonPropertyName("model_variant_slug")]
    public string ModelVariantSlug { get; set; }

    [JsonPropertyName("model_variant_permaslug")]
    public string ModelVariantPermaslug { get; set; }

    [JsonPropertyName("adapter_name")]
    public string AdapterName { get; set; }

    [JsonPropertyName("provider_name")]
    public string ProviderName { get; set; }

    [JsonPropertyName("provider_info")]
    public ProviderInfo ProviderInfo { get; set; }

    [JsonPropertyName("provider_display_name")]
    public string ProviderDisplayName { get; set; }

    [JsonPropertyName("provider_slug")]
    public string ProviderSlug { get; set; }

    [JsonPropertyName("provider_model_id")]
    public string ProviderModelId { get; set; }

    [JsonPropertyName("quantization")]
    public string Quantization { get; set; }

    [JsonPropertyName("variant")]
    public string Variant { get; set; }

    [JsonPropertyName("is_free")]
    public bool IsFree { get; set; }

    [JsonPropertyName("can_abort")]
    public bool CanAbort { get; set; }

    [JsonPropertyName("max_prompt_tokens")]
    public int? MaxPromptTokens { get; set; }

    [JsonPropertyName("max_completion_tokens")]
    public int MaxCompletionTokens { get; set; }

    [JsonPropertyName("max_tokens_per_image")]
    public int? MaxTokensPerImage { get; set; }

    [JsonPropertyName("supported_parameters")]
    public List<string> SupportedParameters { get; set; }

    [JsonPropertyName("is_byok")]
    public bool IsByok { get; set; }

    [JsonPropertyName("moderation_required")]
    public bool ModerationRequired { get; set; }

    [JsonPropertyName("data_policy")]
    public DataPolicy DataPolicy { get; set; }

    [JsonPropertyName("pricing")]
    public Pricing Pricing { get; set; }

    [JsonPropertyName("variable_pricings")]
    public List<VariablePricing> VariablePricings { get; set; }

    [JsonPropertyName("is_hidden")]
    public bool IsHidden { get; set; }

    [JsonPropertyName("is_deranked")]
    public bool IsDeranked { get; set; }

    [JsonPropertyName("is_disabled")]
    public bool IsDisabled { get; set; }

    [JsonPropertyName("supports_tool_parameters")]
    public bool SupportsToolParameters { get; set; }

    [JsonPropertyName("supports_reasoning")]
    public bool SupportsReasoning { get; set; }

    [JsonPropertyName("supports_multipart")]
    public bool SupportsMultipart { get; set; }

    [JsonPropertyName("limit_rpm")]
    public int? LimitRpm { get; set; }

    [JsonPropertyName("limit_rpd")]
    public int? LimitRpd { get; set; }

    [JsonPropertyName("limit_rpm_cf")]
    public int? LimitRpmCf { get; set; }

    [JsonPropertyName("has_completions")]
    public bool HasCompletions { get; set; }

    [JsonPropertyName("has_chat_completions")]
    public bool HasChatCompletions { get; set; }

    [JsonPropertyName("features")]
    public EndpointFeatures Features { get; set; }

    [JsonPropertyName("provider_region")]
    public string ProviderRegion { get; set; }

    [JsonPropertyName("deprecation_date")]
    public DateTime? DeprecationDate { get; set; }
}
