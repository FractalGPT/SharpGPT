namespace FractalGPT.SharpGPTLib.Core.Models.Providers.ImageGeneration;

/// <summary>
/// Ответ генератора изображений
/// </summary>
public class ImageGenerationAnswer
{
    /// <summary>
    /// Если true, то все успешно
    /// </summary>
    public bool StatusOK { get; set; }

    /// <summary>
    /// Сопроводительный текст или текст ошибки
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Байтовое представление изображения
    /// </summary>
    public byte[] ImageData { get; set; }

    public ImageGenerationAnswer(string errorText)
    {
        StatusOK = false;
        Text = errorText;
    }

    public ImageGenerationAnswer(byte[] imageData, string text = null)
    {
        ImageData = imageData;
        Text = text;
        StatusOK = true;
    }
}
