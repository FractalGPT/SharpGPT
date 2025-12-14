using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.OpenRouter.ContextFormat.Models;

// Информация о модели
public class ModelInfo
{
    [JsonPropertyName("slug")]
    public string Slug { get; set; }

    [JsonPropertyName("hf_slug")]
    public string HfSlug { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("hf_updated_at")]
    public DateTime? HfUpdatedAt { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("short_name")]
    public string ShortName { get; set; }

    [JsonPropertyName("author")]
    public string Author { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("model_version_group_id")]
    public string ModelVersionGroupId { get; set; }

    [JsonPropertyName("context_length")]
    public int ContextLength { get; set; }

    [JsonPropertyName("input_modalities")]
    public List<string> InputModalities { get; set; }

    [JsonPropertyName("output_modalities")]
    public List<string> OutputModalities { get; set; }

    [JsonPropertyName("has_text_output")]
    public bool HasTextOutput { get; set; }

    [JsonPropertyName("group")]
    public string Group { get; set; }

    [JsonPropertyName("instruct_type")]
    public string InstructType { get; set; }

    [JsonPropertyName("default_system")]
    public string DefaultSystem { get; set; }

    [JsonPropertyName("default_stops")]
    public List<string> DefaultStops { get; set; }

    [JsonPropertyName("hidden")]
    public bool Hidden { get; set; }

    [JsonPropertyName("router")]
    public string Router { get; set; }

    [JsonPropertyName("warning_message")]
    public string WarningMessage { get; set; }

    [JsonPropertyName("promotion_message")]
    public string PromotionMessage { get; set; }

    [JsonPropertyName("routing_error_message")]
    public string RoutingErrorMessage { get; set; }

    [JsonPropertyName("permaslug")]
    public string Permaslug { get; set; }

    [JsonPropertyName("reasoning_config")]
    public object ReasoningConfig { get; set; }

    [JsonPropertyName("features")]
    public object Features { get; set; }

    [JsonPropertyName("default_parameters")]
    public DefaultParameters DefaultParameters { get; set; }

    [JsonPropertyName("default_order")]
    public List<string> DefaultOrder { get; set; }

    [JsonPropertyName("quick_start_example_type")]
    public string QuickStartExampleType { get; set; }

    [JsonPropertyName("is_trainable_text")]
    public bool? IsTrainableText { get; set; }

    [JsonPropertyName("is_trainable_image")]
    public bool? IsTrainableImage { get; set; }

    [JsonPropertyName("endpoint")]
    public Endpoint Endpoint { get; set; }
}
