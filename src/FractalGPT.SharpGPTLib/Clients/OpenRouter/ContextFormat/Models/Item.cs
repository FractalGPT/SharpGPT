using System.Text.Json;
using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.OpenRouter.ContextFormat.Models;

// Элемент контента
public class Item
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("messageId")]
    public string MessageId { get; set; }

    [JsonPropertyName("data")]
    public JsonElement Data { get; set; } // Используем JsonElement для гибкости
}
