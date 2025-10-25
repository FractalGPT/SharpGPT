using System.Text.Json;
using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Core.Models.Common.Messages.Content;

/// <summary>
/// Custom JSON converter for handling polymorphic content (string or MessageContent).
/// </summary>
public class ContentJsonConverter : JsonConverter<object>
{
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var rootElement = jsonDoc.RootElement;

        if (rootElement.ValueKind == JsonValueKind.String)
        {
            return rootElement.GetString()!;
        }

        if (rootElement.ValueKind == JsonValueKind.Array)
        {
            return JsonSerializer.Deserialize<MessageContent>(rootElement.GetRawText(), options)!;
        }

        throw new JsonException("Content must be a string or an array of content items.");
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        if (value is string stringContent)
        {
            writer.WriteStringValue(stringContent);
        }
        else if (value is MessageContent messageContent)
        {
            JsonSerializer.Serialize(writer, messageContent, options);
        }
        else
        {
            throw new JsonException("Content must be a string or MessageContent.");
        }
    }
}
