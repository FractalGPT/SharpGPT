using System.Text.RegularExpressions;
using FractalGPT.SharpGPTLib.API.LLMAPI;

namespace FractalGPT.SharpGPTLib.LLM;

/// <summary>
/// Классификатор на базе LLM с использованием вероятностей токенов.
/// </summary>
public class ClassifierWithLLM
{
    /// <summary>
    /// Апи для отправки запросов на LLM по стандарту OpenAI (также поддерживается DeepSeek, VLLM, OpenRouter, Replicate и тп.)
    /// </summary>
    public ChatLLMApi ChatLLMApi { get; set; }

    protected HashSet<string> TokenValues { get; set; } // Разрешённые токены (case-insensitive после очистки)

    protected int ClTokenPosition { get; set; } // Позиция токена класса в генерируемом выводе

    public string ClPrompt { get; set; } = "{{input_text}}"; // Шаблон промпта

    public string AnswerStart { get; set; } = "";

    public bool CleanTokens { get; set; } = true; // Флаг для очистки токенов от пунктуации

    /// <summary>
    /// Конструктор по умолчанию.
    /// </summary>
    public ClassifierWithLLM() { }

    /// <summary>
    /// Конструктор с параметрами.
    /// </summary>
    /// <param name="chatLLMApi">API для LLM.</param>
    /// <param name="tokenValues">Разрешённые значения токенов (null для дефолтных 0-9).</param>
    /// <param name="clTokenPosition">Позиция токена класса (должна быть >= 0).</param>
    public ClassifierWithLLM(ChatLLMApi chatLLMApi, IEnumerable<string> tokenValues = null, int clTokenPosition = 0)
    {
        if (clTokenPosition < 0) throw new ArgumentException("clTokenPosition must be >= 0.");

        ChatLLMApi = chatLLMApi ?? throw new ArgumentNullException(nameof(chatLLMApi));
        TokenValues = tokenValues == null
            ? GetBaseTokenCls()
            : new HashSet<string>(tokenValues.Select(t => ClearStr(t).ToLower()), StringComparer.OrdinalIgnoreCase);
        ClTokenPosition = clTokenPosition;
    }

    /// <summary>
    /// Асинхронный метод классификации текста.
    /// </summary>
    /// <param name="inputText">Текст для классификации.</param>
    /// <param name="topk">Количество топ-вариантов logprobs (default=10).</param>
    /// <param name="genTemperature">Температура генерации LLM (default=0.2 для детерминизма).</param>
    /// <param name="softmaxTemperature">Температура для softmax (default=5.0 для стандартного поведения).</param>
    /// <returns>Список классов с вероятностями, отсортированный по убыванию.</returns>
    public async Task<List<ClassifyData>> TextClassifyAsync(string inputText, int topk = 10, double genTemperature = 0.2, double softmaxTemperature = 5.0)
    {
        if (string.IsNullOrEmpty(inputText)) throw new ArgumentException("inputText cannot be null or empty.");

        // Экранирование плейсхолдера для безопасности
        string inputSafe = inputText.Replace("{{input_text}}", "[input_text]");
        string inputWithPrompt = ClPrompt.Replace("{{input_text}}", inputSafe);

        GenerateSettings generateSettings = new GenerateSettings()
        {
            Temperature = genTemperature,
            MinTokens = ClTokenPosition + 1,
            MaxTokens = ClTokenPosition + 1,
            LogProbs = true,
            TopLogprobs = topk
        };

        try
        {
            var llmAnswer = await ChatLLMApi.SendWithoutContextWithStartAsync(inputWithPrompt, AnswerStart, generateSettings);
            var logProbs = llmAnswer.Choices[0].Logprobs.Content[ClTokenPosition].TopLogprobs; // Logprobs на позиции

            // Вычисление softmax
            var classifyDatas = Softmax(logProbs, softmaxTemperature);

            // Сортировка по вероятности descending
            classifyDatas.Sort((a, b) => b.Prob.CompareTo(a.Prob));

            return classifyDatas;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error during LLM classification.", ex);
        }
    }

    /// <summary>
    /// Очистка строки от пунктуации и пробелов (trim)
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private string ClearStr(string str)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;

        // Удаляем все символы кроме английских/русских букв и цифр с начала и конца
        return Regex.Replace(str, @"^[^a-zA-Zа-яА-ЯёЁ0-9]+|[^a-zA-Zа-яА-ЯёЁ0-9]+$", "").ToLower();
    }

    // Дефолтные токены (0-9 для шкалы)
    private HashSet<string> GetBaseTokenCls() => new HashSet<string>(Enumerable.Range(0, 10).Select(i => i.ToString()), StringComparer.OrdinalIgnoreCase);

    // Softmax с фильтрацией и объединением
    private List<ClassifyData> Softmax(List<TopLogprob> topLogprobs, double temperature)
    {
        if (temperature <= 0) throw new ArgumentException("softmaxTemperature must be > 0.");

        // Преобразование: (token, exp(logprob / temp))
        var expProbs = topLogprobs
            .Select(lp => (Token: CleanTokens ? ClearStr(lp.Token) : lp.Token.ToLower(), ExpProb: Math.Exp(lp.Logprob / temperature)))
            .Where(t => !string.IsNullOrEmpty(t.Token) && TokenValues.Contains(t.Token))
            .GroupBy(t => t.Token) // Объединение дубликатов
            .Select(g => (Token: g.Key, ExpProb: g.Sum(t => t.ExpProb)))
            .ToList();

        double sumExp = expProbs.Sum(t => t.ExpProb);
        if (sumExp <= double.Epsilon) return new List<ClassifyData>(); // Нет валидных токенов

        return expProbs.Select(t => new ClassifyData { ClToken = t.Token, Prob = t.ExpProb / sumExp }).ToList();
    }
}

/// <summary>
/// Данные классификации.
/// </summary>
public class ClassifyData
{
    /// <summary>
    /// Токен класса.
    /// </summary>
    public string ClToken { get; set; }

    /// <summary>
    /// Вероятность (0-1).
    /// </summary>
    public double Prob { get; set; }
}