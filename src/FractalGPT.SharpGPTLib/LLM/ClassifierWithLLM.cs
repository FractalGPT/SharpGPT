using FractalGPT.SharpGPTLib.API.LLMAPI;
using System.Text.RegularExpressions;

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


//using FractalGPT.SharpGPTLib.API.LLMAPI;
//using System.Text.RegularExpressions;

//namespace FractalGPT.SharpGPTLib.LLM;

///// <summary>
///// Классификатор на базе LLM с использованием вероятностей токенов.
///// Поддерживает как однозначные (0-9), так и многозначные индексы (10+).
///// </summary>
//public class ClassifierWithLLM
//{
//    /// <summary>
//    /// Апи для отправки запросов на LLM по стандарту OpenAI
//    /// </summary>
//    public ChatLLMApi ChatLLMApi { get; set; }

//    /// <summary>
//    /// Разрешённые токены (case-insensitive после очистки)
//    /// </summary>
//    protected HashSet<string> TokenValues { get; set; }

//    /// <summary>
//    /// Позиция начала токена класса в генерируемом выводе (0-based)
//    /// </summary>
//    protected int ClTokenPosition { get; set; }

//    /// <summary>
//    /// Максимальная длина индекса в токенах (default=1 для 0-9, 2 для 10-99)
//    /// </summary>
//    public int MaxIndexLength { get; set; } = 1;

//    /// <summary>
//    /// Шаблон промпта с плейсхолдером {{input_text}}
//    /// </summary>
//    public string ClPrompt { get; set; } = "{{input_text}}";

//    /// <summary>
//    /// Начало ответа модели (префикс перед индексом класса)
//    /// </summary>
//    public string AnswerStart { get; set; } = "";

//    /// <summary>
//    /// Флаг для очистки токенов от пунктуации
//    /// </summary>
//    public bool CleanTokens { get; set; } = true;

//    /// <summary>
//    /// Использовать ли beam search для многозначных индексов (медленнее, но точнее)
//    /// </summary>
//    public bool UseBeamSearch { get; set; } = false;

//    /// <summary>
//    /// Ширина луча для beam search (количество лучших путей)
//    /// </summary>
//    public int BeamWidth { get; set; } = 5;

//    /// <summary>
//    /// Конструктор по умолчанию.
//    /// </summary>
//    public ClassifierWithLLM() { }

//    /// <summary>
//    /// Конструктор с параметрами.
//    /// </summary>
//    public ClassifierWithLLM(ChatLLMApi chatLLMApi, IEnumerable<string> tokenValues = null, int clTokenPosition = 0, int maxIndexLength = 1)
//    {
//        if (clTokenPosition < 0) throw new ArgumentException("clTokenPosition must be >= 0.");
//        if (maxIndexLength < 1) throw new ArgumentException("maxIndexLength must be >= 1.");

//        ChatLLMApi = chatLLMApi ?? throw new ArgumentNullException(nameof(chatLLMApi));
//        TokenValues = tokenValues == null
//            ? GetBaseTokenCls()
//            : new HashSet<string>(tokenValues.Select(t => ClearStr(t).ToLower()), StringComparer.OrdinalIgnoreCase);
//        ClTokenPosition = clTokenPosition;
//        MaxIndexLength = maxIndexLength;
//    }

//    /// <summary>
//    /// Асинхронный метод классификации текста с поддержкой многозначных индексов.
//    /// </summary>
//    public async Task<List<ClassifyData>> TextClassifyAsync(string inputText, int topk = 10, double genTemperature = 0.2, double softmaxTemperature = 5.0)
//    {
//        if (string.IsNullOrEmpty(inputText)) throw new ArgumentException("inputText cannot be null or empty.");

//        string inputSafe = inputText.Replace("{{input_text}}", "[input_text]");
//        string inputWithPrompt = ClPrompt.Replace("{{input_text}}", inputSafe);

//        if (MaxIndexLength == 1)
//        {
//            // Однозначный индекс - простой случай
//            return await ClassifySingleDigitAsync(inputWithPrompt, topk, genTemperature, softmaxTemperature);
//        }
//        else
//        {
//            // Многозначный индекс - используем beam search или greedy подход
//            if (UseBeamSearch)
//            {
//                return await ClassifyMultiDigitBeamSearchAsync(inputWithPrompt, topk, genTemperature, softmaxTemperature);
//            }
//            else
//            {
//                return await ClassifyMultiDigitGreedyAsync(inputWithPrompt, topk, genTemperature, softmaxTemperature);
//            }
//        }
//    }

//    /// <summary>
//    /// Классификация для однозначных индексов (0-9)
//    /// </summary>
//    private async Task<List<ClassifyData>> ClassifySingleDigitAsync(string inputWithPrompt, int topk, double genTemperature, double softmaxTemperature)
//    {
//        GenerateSettings generateSettings = new GenerateSettings()
//        {
//            Temperature = genTemperature,
//            MinTokens = ClTokenPosition + 1,
//            MaxTokens = ClTokenPosition + 1,
//            LogProbs = true,
//            TopLogprobs = topk
//        };

//        var llmAnswer = await ChatLLMApi.SendWithoutContextWithStartAsync(inputWithPrompt, AnswerStart, generateSettings);
//        var logProbs = llmAnswer.Choices[0].Logprobs.Content[ClTokenPosition].TopLogprobs;

//        var classifyDatas = Softmax(logProbs, softmaxTemperature);
//        classifyDatas.Sort((a, b) => b.Prob.CompareTo(a.Prob));

//        return classifyDatas;
//    }

//    /// <summary>
//    /// Greedy подход: генерируем все позиции сразу, берём только наиболее вероятный путь
//    /// Быстро, но менее точно для многозначных индексов
//    /// </summary>
//    private async Task<List<ClassifyData>> ClassifyMultiDigitGreedyAsync(string inputWithPrompt, int topk, double genTemperature, double softmaxTemperature)
//    {
//        GenerateSettings generateSettings = new GenerateSettings()
//        {
//            Temperature = genTemperature,
//            MinTokens = ClTokenPosition + MaxIndexLength,
//            MaxTokens = ClTokenPosition + MaxIndexLength,
//            LogProbs = true,
//            TopLogprobs = topk
//        };

//        var llmAnswer = await ChatLLMApi.SendWithoutContextWithStartAsync(inputWithPrompt, AnswerStart, generateSettings);
//        var logprobsContent = llmAnswer.Choices[0].Logprobs.Content;

//        // Собираем наиболее вероятные комбинации
//        var results = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

//        // Берём топ-1 для первой позиции
//        var firstPosLogprobs = logprobsContent[ClTokenPosition].TopLogprobs;
//        var firstPosProbs = Softmax(firstPosLogprobs, softmaxTemperature)
//            .Where(cd => IsDigit(cd.ClToken))
//            .OrderByDescending(cd => cd.Prob)
//            .Take(BeamWidth)
//            .ToList();

//        foreach (var firstDigit in firstPosProbs)
//        {
//            string combination = firstDigit.ClToken;
//            double probability = firstDigit.Prob;

//            // Для каждой следующей позиции берём условную вероятность
//            for (int pos = 1; pos < MaxIndexLength; pos++)
//            {
//                int tokenPos = ClTokenPosition + pos;
//                if (tokenPos >= logprobsContent.Count) break;

//                var posLogprobs = logprobsContent[tokenPos].TopLogprobs;
//                var posProbs = Softmax(posLogprobs, softmaxTemperature)
//                    .Where(cd => IsDigit(cd.ClToken))
//                    .OrderByDescending(cd => cd.Prob)
//                    .FirstOrDefault();

//                if (posProbs != null)
//                {
//                    combination += posProbs.ClToken;
//                    probability *= posProbs.Prob; // Условная вероятность P(digit_i | digit_0...digit_{i-1})
//                }
//            }

//            // Фильтруем только разрешённые значения
//            if (TokenValues.Contains(combination))
//            {
//                if (results.ContainsKey(combination))
//                    results[combination] += probability;
//                else
//                    results[combination] = probability;
//            }
//        }

//        // Нормализуем и сортируем
//        return NormalizeAndSort(results);
//    }

//    /// <summary>
//    /// Beam Search: генерируем цифру за цифрой, поддерживая несколько лучших путей
//    /// Медленнее, но точнее учитывает условные вероятности
//    /// </summary>
//    private async Task<List<ClassifyData>> ClassifyMultiDigitBeamSearchAsync(string inputWithPrompt, int topk, double genTemperature, double softmaxTemperature)
//    {
//        var beamCandidates = new List<BeamCandidate>
//        {
//            new BeamCandidate { Prefix = AnswerStart, Probability = 1.0 }
//        };

//        for (int digitPos = 0; digitPos < MaxIndexLength; digitPos++)
//        {
//            var newCandidates = new List<BeamCandidate>();

//            foreach (var candidate in beamCandidates)
//            {
//                // Генерируем следующий токен с учётом текущего префикса
//                GenerateSettings generateSettings = new GenerateSettings()
//                {
//                    Temperature = genTemperature,
//                    MinTokens = ClTokenPosition + digitPos + 1,
//                    MaxTokens = ClTokenPosition + digitPos + 1,
//                    LogProbs = true,
//                    TopLogprobs = topk
//                };

//                var llmAnswer = await ChatLLMApi.SendWithoutContextWithStartAsync(inputWithPrompt, candidate.Prefix, generateSettings);
//                var logProbs = llmAnswer.Choices[0].Logprobs.Content[ClTokenPosition + digitPos].TopLogprobs;

//                var digitProbs = Softmax(logProbs, softmaxTemperature)
//                    .Where(cd => IsDigit(cd.ClToken))
//                    .ToList();

//                foreach (var digitProb in digitProbs)
//                {
//                    // Извлекаем только цифровую часть из префикса
//                    string digitSequence = ExtractDigits(candidate.Prefix.Replace(AnswerStart, "")) + digitProb.ClToken;

//                    newCandidates.Add(new BeamCandidate
//                    {
//                        Prefix = candidate.Prefix + digitProb.ClToken,
//                        DigitSequence = digitSequence,
//                        Probability = candidate.Probability * digitProb.Prob // P(d0) * P(d1|d0) * P(d2|d0,d1) ...
//                    });
//                }
//            }

//            // Оставляем только TopK лучших кандидатов
//            beamCandidates = newCandidates
//                .OrderByDescending(c => c.Probability)
//                .Take(BeamWidth)
//                .ToList();
//        }

//        // Собираем финальные результаты
//        var results = beamCandidates
//            .Where(c => TokenValues.Contains(c.DigitSequence))
//            .GroupBy(c => c.DigitSequence)
//            .ToDictionary(g => g.Key, g => g.Sum(c => c.Probability), StringComparer.OrdinalIgnoreCase);

//        return NormalizeAndSort(results);
//    }

//    /// <summary>
//    /// Извлекает только цифры из строки
//    /// </summary>
//    private string ExtractDigits(string str)
//    {
//        return new string(str.Where(char.IsDigit).ToArray());
//    }

//    /// <summary>
//    /// Нормализует вероятности и сортирует по убыванию
//    /// </summary>
//    private List<ClassifyData> NormalizeAndSort(Dictionary<string, double> results)
//    {
//        double totalProb = results.Sum(kvp => kvp.Value);

//        var classifyDatas = results
//            .Select(kvp => new ClassifyData
//            {
//                ClToken = kvp.Key,
//                Prob = totalProb > double.Epsilon ? kvp.Value / totalProb : 0
//            })
//            .ToList();

//        classifyDatas.Sort((a, b) => b.Prob.CompareTo(a.Prob));
//        return classifyDatas;
//    }

//    /// <summary>
//    /// Проверка, является ли строка одной цифрой
//    /// </summary>
//    private bool IsDigit(string str)
//    {
//        return !string.IsNullOrEmpty(str) && str.Length == 1 && char.IsDigit(str[0]);
//    }

//    /// <summary>
//    /// Очистка строки от пунктуации и пробелов
//    /// </summary>
//    private string ClearStr(string str)
//    {
//        if (string.IsNullOrEmpty(str))
//            return string.Empty;

//        return Regex.Replace(str, @"^[^a-zA-Zа-яА-ЯёЁ0-9]+|[^a-zA-Zа-яА-ЯёЁ0-9]+$", "").ToLower();
//    }

//    /// <summary>
//    /// Дефолтные токены (0-9)
//    /// </summary>
//    private HashSet<string> GetBaseTokenCls() => new HashSet<string>(
//        Enumerable.Range(0, 10).Select(i => i.ToString()),
//        StringComparer.OrdinalIgnoreCase);

//    /// <summary>
//    /// Автоматическое определение максимальной длины индекса
//    /// </summary>
//    public static int DetermineMaxIndexLength(IEnumerable<string> tokenValues)
//    {
//        if (tokenValues == null || !tokenValues.Any())
//            return 1;

//        return tokenValues
//            .Where(t => !string.IsNullOrEmpty(t) && t.All(char.IsDigit))
//            .Select(t => t.Length)
//            .DefaultIfEmpty(1)
//            .Max();
//    }

//    /// <summary>
//    /// Softmax с фильтрацией и объединением
//    /// </summary>
//    private List<ClassifyData> Softmax(List<TopLogprob> topLogprobs, double temperature)
//    {
//        if (temperature <= 0) throw new ArgumentException("softmaxTemperature must be > 0.");

//        var expProbs = topLogprobs
//            .Select(lp => (Token: CleanTokens ? ClearStr(lp.Token) : lp.Token.ToLower(), ExpProb: Math.Exp(lp.Logprob / temperature)))
//            .Where(t => !string.IsNullOrEmpty(t.Token))
//            .GroupBy(t => t.Token)
//            .Select(g => (Token: g.Key, ExpProb: g.Sum(t => t.ExpProb)))
//            .ToList();

//        double sumExp = expProbs.Sum(t => t.ExpProb);
//        if (sumExp <= double.Epsilon) return new List<ClassifyData>();

//        return expProbs.Select(t => new ClassifyData { ClToken = t.Token, Prob = t.ExpProb / sumExp }).ToList();
//    }
//}

///// <summary>
///// Кандидат для beam search
///// </summary>
//internal class BeamCandidate
//{
//    public string Prefix { get; set; } = "";
//    public string DigitSequence { get; set; } = "";
//    public double Probability { get; set; } = 1.0;
//}

///// <summary>
///// Данные классификации
///// </summary>
//public class ClassifyData
//{
//    public string ClToken { get; set; }
//    public double Prob { get; set; }
//    public override string ToString() => $"{ClToken}: {Prob:P2}";
//}