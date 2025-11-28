using System.Text;

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

