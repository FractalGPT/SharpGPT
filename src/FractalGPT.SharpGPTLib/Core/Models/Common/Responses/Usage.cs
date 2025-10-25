using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Core.Models.Common.Responses;


/// <summary>
/// Represents token usage information for a given request or session.
/// </summary>
[Serializable]
public class Usage
{
    /// <summary>
    /// Gets or sets the number of tokens used in the prompt.
    /// </summary>
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }

    /// <summary>
    /// Gets or sets the number of tokens used by the model's completion.
    /// </summary>
    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }

    /// <summary>
    /// Gets or sets the total number of tokens used in the session.
    /// </summary>
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }

    [JsonPropertyName("reasoning_tokens")]
    public int ReasoningTokens { get; set; }

    [JsonPropertyName("cost")]
    public object Cost { get; set; }
}

public static class CostExtractor
{
    public static decimal? TryExtract(object cost)
    {
        // 1. Проверяем на null в самом начале
        if (cost == null)
        {
            return null;
        }

        if (cost is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Object)
        {
            // Имя свойства в JSON - "total_cost", а не "TotalCost".
            if (jsonElement.TryGetProperty("total_cost", out var totalCostProperty))
            {
                // Пытаемся получить значение как decimal
                if (totalCostProperty.TryGetDecimal(out decimal decimalValue))
                {
                    return decimalValue;
                }
            }
        }

        // 3. Пытаемся преобразовать в decimal из любого другого типа.
        // Этот метод безопасен и не выбрасывает исключений.
        // Он справится с int, long, double, string и самим decimal.
        // Используем CultureInfo.InvariantCulture, чтобы избежать проблем с разделителями (точка вместо запятой).
        if (decimal.TryParse(cost.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
        {
            return result;
        }

        // 4. Если ничего не подошло, возвращаем null
        return null;
    }
}

