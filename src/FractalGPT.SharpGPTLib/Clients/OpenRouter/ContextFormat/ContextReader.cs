using FractalGPT.SharpGPTLib.Clients.OpenRouter.ContextFormat.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Clients.OpenRouter.ContextFormat;

// Класс для чтения файла
public class ContextReader
{
    public static OpenRouterContext ReadFromFile(string filePath)
    {
        var json = File.ReadAllText(filePath);
        return ReadFromJson(json);
    }

    public static OpenRouterContext ReadFromJson(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        return JsonSerializer.Deserialize<OpenRouterContext>(json, options);
    }

    public static async Task<OpenRouterContext> ReadFromFileAsync(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        return await JsonSerializer.DeserializeAsync<OpenRouterContext>(stream, options);
    }
}
