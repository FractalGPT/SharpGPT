using FractalGPT.SharpGPTLib.Core.Models.Common.Messages;
using FractalGPT.SharpGPTLib.Core.Models.Common.Requests;
using FractalGPT.SharpGPTLib.Utilities.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.API.LLMAPI;

/// <summary>
/// Представляет данные, отправляемые в LLM (большую языковую модель), включая сообщения и параметры запроса
/// </summary>
// SendDataLLM.cs - Основная часть с бизнес-логикой
public partial class SendDataLLM
{
    /// <summary>
    /// Инициализирует новый экземпляр класса
    /// </summary>
    public SendDataLLM(string modelName, GenerateSettings generateSettings = null)
    {
        if (string.IsNullOrWhiteSpace(modelName))
            throw new ArgumentNullException(nameof(modelName),
                "Название модели не может быть null или пустой строкой.");

        generateSettings ??= new();

        // Инициализация всех свойств
        ModelName = modelName;
        Temperature = generateSettings.Temperature;
        TopK = generateSettings.TopK;
        TopP = generateSettings.TopP;
        RepetitionPenalty = generateSettings.RepetitionPenalty;
        MaxTokens = generateSettings.MaxTokens;
        MinTokens = generateSettings.MinTokens;
        Stream = generateSettings.Stream;
        ReasoningSettings = generateSettings.ReasoningSettings;
        ReasoningEffort = generateSettings?.ReasoningSettings?.Enabled == true
            ? "medium"
            : "none";
        LogProbs = generateSettings.LogProbs;
        TopLogprobs = generateSettings.TopLogprobs;
        Messages = new List<LLMMessage>();
    }

    /// <summary>
    /// Загружает список сообщений в диалог
    /// </summary>
    public void SetMessages(IEnumerable<LLMMessage> messages)
        => Messages = messages.FixContext();

    /// <summary>
    /// Сериализует текущий объект в строку JSON
    /// </summary>
    public string GetJson(bool writeIndented = false)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = writeIndented,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        return JsonSerializer.Serialize(this, options);
    }
}