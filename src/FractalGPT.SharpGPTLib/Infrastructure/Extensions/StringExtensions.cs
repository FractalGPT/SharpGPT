using System.Text;
using System.Text.RegularExpressions;

namespace FractalGPT.SharpGPTLib.Infrastructure.Extensions;

/// <summary>
/// Extension методы для работы со строками
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Обрезает строку до указанной длины и заменяет повторяющиеся символы (более 10 раз подряд) на краткую форму
    /// </summary>
    /// <param name="text">Исходная строка</param>
    /// <param name="maxLength">Максимальная длина (по умолчанию 2000)</param>
    /// <param name="repeatThreshold">Порог повторений для замены (по умолчанию 10)</param>
    /// <returns>Обработанная строка</returns>
    public static string TruncateForLogging(this string text, int maxLength = 2000, int repeatThreshold = 10)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        if (maxLength <= 1)
            throw new ArgumentException(nameof(maxLength));
        if (repeatThreshold <= 1)
            throw new ArgumentException(nameof(repeatThreshold));

        // Сначала вырезаем base64-картинки: иначе они занимают весь лимит лога
        // и вместо текста запроса в логе оказывается тело изображения
        text = ReplaceBase64Payloads(text);

        // Берём первые 10000 символов для обработки (если строка длиннее)
        int bufferSize = Math.Min(maxLength * 5, text.Length);
        var workingText = text.Substring(0, bufferSize);

        // Заменяем повторяющиеся символы
        var processedText = ReplaceRepeatingCharacters(workingText, repeatThreshold);

        // Обрезаем до финальной длины
        if (processedText.Length > maxLength)
            processedText = processedText.Substring(0, maxLength);
        
        return processedText;
    }

    private static readonly Regex Base64PayloadRegex = new(
        @"(data:[^;""]{0,80};base64,)([A-Za-z0-9+/=]{200,})",
        RegexOptions.None,
        TimeSpan.FromSeconds(2));

    /// <summary>
    /// Заменяет тело base64-изображения на краткую пометку с размером.
    /// Например: "data:image/png;base64,iVBORw0..." -> "data:image/png;base64,[изображение, 5412340 символов]"
    /// </summary>
    private static string ReplaceBase64Payloads(string text)
    {
        if (text.IndexOf(";base64,", StringComparison.OrdinalIgnoreCase) < 0)
            return text;

        try
        {
            // Группа 1 - префикс "data:...;base64,", группа 2 - само base64-тело
            return Base64PayloadRegex.Replace(
                text,
                match => $"{match.Groups[1].Value}[изображение, {match.Groups[2].Value.Length} символов]");
        }
        catch (RegexMatchTimeoutException)
        {
            // Метод вызывается из catch-блоков при логировании ошибок -
            // он не имеет права бросать исключение и подменять исходную ошибку
            return text;
        }
    }

    /// <summary>
    /// Заменяет повторяющиеся символы на краткую форму
    /// Например: "aaaaaaaaaaaa" -> "a[повторяется 12 раз]"
    /// </summary>
    private static string ReplaceRepeatingCharacters(string text, int threshold)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= threshold)
            return text ?? string.Empty;

        var result = new StringBuilder();
        int count = 1;
        char currentChar = text[0];

        for (int i = 1; i < text.Length; i++)
        {
            if (text[i] == currentChar)
            {
                count++;
            }
            else
            {
                // Записываем предыдущий символ
                AppendCharacter(result, currentChar, count, threshold);
                currentChar = text[i];
                count = 1;
            }
        }

        // Не забываем последний символ
        AppendCharacter(result, currentChar, count, threshold);

        return result.ToString();
    }

    /// <summary>
    /// Добавляет символ в результат с учетом порога повторений
    /// </summary>
    private static void AppendCharacter(StringBuilder builder, char character, int count, int threshold)
    {
        if (count > threshold)
        {
            // Показываем символ и количество повторений
            builder.Append(character);
            builder.Append($"[повторяется {count} раз]");
        }
        else
        {
            // Добавляем символ count раз
            builder.Append(character, count);
        }
    }
}

