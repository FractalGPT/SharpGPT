using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.API.LLMAPI;

public class UsageCost
{
    [JsonPropertyName("input_tokens_cost")]
    public decimal InputTokensCost { get; set; }

    [JsonPropertyName("output_tokens_cost")]
    public decimal OutputTokensCost { get; set; }

    [JsonPropertyName("citation_tokens_cost")]
    public decimal CitationTokensCost { get; set; }

    [JsonPropertyName("reasoning_tokens_cost")]
    public decimal ReasoningTokensCost { get; set; }

    [JsonPropertyName("search_queries_cost")]
    public decimal SearchQueriesCost { get; set; }

    [JsonPropertyName("total_cost")]
    public decimal TotalCost { get; set; }
}
