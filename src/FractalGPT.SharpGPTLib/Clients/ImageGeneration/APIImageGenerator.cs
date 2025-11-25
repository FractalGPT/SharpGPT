using FractalGPT.SharpGPTLib.Clients.Base;
using FractalGPT.SharpGPTLib.Core.Models.Common.Messages;
using FractalGPT.SharpGPTLib.Core.Models.Common.Responses;
using FractalGPT.SharpGPTLib.Core.Models.Providers.ImageGeneration;

namespace FractalGPT.SharpGPTLib.Clients.ImageGeneration;

/// <summary>
/// Генерация изображений
/// </summary>
[Serializable]
public class APIImageGenerator
{
    private readonly ChatLLMApi _imageGenerativeModelApi;

    public APIImageGenerator(ChatLLMApi llmApi)
    {

        _imageGenerativeModelApi = llmApi;
    }


    public async Task<ImageGenerationAnswer> GenerateAsync(string prompt) =>
        await GenerateAsync(new LLMMessage(LLMMessage.UserRole, prompt));

    public async Task<ImageGenerationAnswer> GenerateAsync(LLMMessage prompt)
    {
        List<LLMMessage> context = new List<LLMMessage>() { prompt };
        return await GenerateAsync(context);
    }

    public async Task<ImageGenerationAnswer> GenerateAsync(IEnumerable<LLMMessage> context)
    {
        try
        {
            var response = await _imageGenerativeModelApi.SendWithContextAsync(context);

            var firstChoice = response?.Choices?.FirstOrDefault();
            var message = firstChoice?.Message;
            var imageUrl = message?.Images?.FirstOrDefault()?.ImageUrl?.Url;

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                string errorContent = (string)message?.Content ?? "The API response was empty or malformed.";
                return new ImageGenerationAnswer(errorContent);
            }

            // Извлекаем стоимость из Usage (если доступна)
            var cost = CostExtractor.TryExtract(response?.Usage?.Cost);

            return new ImageGenerationAnswer(ParseDataUri(imageUrl), message?.Content?.ToString(), cost);
        }

        catch (Exception ex)
        {
            return new ImageGenerationAnswer(ex.Message);
        }
    }


    private byte[] ParseDataUri(string dataUri)
    {
        int commaIndex = dataUri.IndexOf(',');
        if (commaIndex == -1)
        {
            throw new FormatException("Invalid data URI: comma separator not found.");
        }

        string metadata = dataUri.Substring(0, commaIndex);
        if (!metadata.Contains(";base64"))
        {
            throw new FormatException("Invalid data URI: ';base64' specifier not found.");
        }

        string base64Data = dataUri.Substring(commaIndex + 1);

        try
        {
            return Convert.FromBase64String(base64Data);
        }
        catch (FormatException ex)
        {
            throw new FormatException("Failed to convert base64 string to byte array. The data might be corrupted.", ex);
        }
    }
}
