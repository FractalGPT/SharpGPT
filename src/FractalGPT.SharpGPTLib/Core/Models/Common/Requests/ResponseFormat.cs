using System.Text.Json;
using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Core.Models.Common.Requests;

/// <summary>
/// Формат ответа LLM — поддержка Structured Output (OpenAI, Gemini, OpenRouter и т.д.)
/// Сериализуется в поле "response_format" запроса.
/// </summary>
public class ResponseFormat
{
    /// <summary>
    /// Тип формата ответа: "json_schema", "json_object" или "text"
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// Определение JSON Schema (только для type = "json_schema")
    /// </summary>
    [JsonPropertyName("json_schema")]
    public JsonSchemaDefinition JsonSchema { get; set; }

    // ============================================================
    // Фабричные методы
    // ============================================================

    /// <summary>
    /// Создает формат Structured Output с JSON Schema (strict)
    /// </summary>
    /// <param name="name">Имя схемы (латиницей, без пробелов)</param>
    /// <param name="schemaJson">JSON-строка с описанием схемы</param>
    /// <param name="strict">Strict mode — гарантирует 100% соответствие схеме</param>
    public static ResponseFormat CreateJsonSchema(string name, string schemaJson, bool strict = true)
    {
        var schemaElement = JsonDocument.Parse(schemaJson).RootElement.Clone();
        return new ResponseFormat
        {
            Type = "json_schema",
            JsonSchema = new JsonSchemaDefinition
            {
                Name = name,
                Strict = strict,
                Schema = schemaElement
            }
        };
    }

    /// <summary>
    /// Создает формат "json_object" (гарантирует валидный JSON, но без схемы)
    /// </summary>
    public static ResponseFormat CreateJsonObject()
    {
        return new ResponseFormat { Type = "json_object" };
    }
}

/// <summary>
/// Описание JSON Schema для Structured Output
/// </summary>
public class JsonSchemaDefinition
{
    /// <summary>
    /// Имя схемы (используется API для кэширования)
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Strict mode — гарантирует 100% соответствие схеме.
    /// Все свойства должны быть в required, additionalProperties: false.
    /// </summary>
    [JsonPropertyName("strict")]
    public bool? Strict { get; set; }

    /// <summary>
    /// Тело JSON Schema
    /// </summary>
    [JsonPropertyName("schema")]
    public JsonElement Schema { get; set; }
}
