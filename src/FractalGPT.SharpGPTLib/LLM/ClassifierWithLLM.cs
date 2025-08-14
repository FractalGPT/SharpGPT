using FractalGPT.SharpGPTLib.API.LLMAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace FractalGPT.SharpGPTLib.LLM
{
    /// <summary>
    /// Классификатор на базе LLM 
    /// </summary>
    public class ClassifierWithLLM
    {
        public ChatLLMApi ChatLLMApi { get; set; }

        protected HashSet<string> TokenValues { get; set; }

        protected int ClTokenPosition { get; set; } // Позиция токена для определения класса

        public string ClPrompt { get; set; } = "{{input_text}}";

        public ClassifierWithLLM() { }

        /// <summary>
        /// Классификатор на базе LLM 
        /// </summary>
        public ClassifierWithLLM(ChatLLMApi chatLLMApi, IEnumerable<string> tokenValues, int clTokenPosition)
        {
            ChatLLMApi = chatLLMApi;
            TokenValues = tokenValues == null ? GetBaseTokenCls() : new HashSet<string>(tokenValues.Select(ClearStr).ToList()); // Значения токенов, без пробелов и знаков припинания

        }

        /// <summary>
        /// Асинхронный метод классификации текстов
        /// </summary>
        /// <param name="inputText">Текст для классификации</param>
        /// <param name="topk">Топ значение</param>
        /// <param name="temperature">Температура</param>
        /// <returns></returns>
        public async Task<List<ClassifyData>> TextClassifyAsync(string inputText, int topk = 5, double temperature = 5) 
        {
            string inputSafe = inputText.Replace("{{input_text}}", "{input_text}"); // Убираем возможность неверной вставки
            string inputWithPrompt = ClPrompt.Replace("{{input_text}}", inputSafe);
            GenerateSettings generateSettings = new GenerateSettings()
            {
                Temperature = 0.2,
                MinTokens = ClTokenPosition+1,
                MaxTokens = ClTokenPosition+2,
                LogProbs = true,
                TopLogprobs = topk
            };


            var llmAnswer = await ChatLLMApi.SendWithoutContextAsync(inputWithPrompt, generateSettings);
            var logProbs = llmAnswer.Choices[0].Logprobs.Content[ClTokenPosition].TopLogprobs; // Получение логарифмов вероятности 

            // Получение вероятностей классов
            return Softmax(logProbs, temperature);
        }

        // Очистка строки
        private string ClearStr(string str) 
            => str.Trim("- —,.!?;:\"'_+{}[]()<>".ToCharArray());

        // Словарь доступных токенов по умолчанию
        private HashSet<string> GetBaseTokenCls() => new HashSet<string>()
            {
                "0","1", "2", "3", "4", "5", "6", "7", "8", "9"
            };


        // Софтмак с фильтрами невозможных токенов и объединением похожих
        private List<ClassifyData> Softmax(List<TopLogprob> topLogprobs, double temperature) 
        {
            var topExp = topLogprobs.Select(x => 
                new TopLogprob(ClearStr(x.Token), 
                Math.Exp(x.Logprob / temperature), 
                x.Bytes)).ToList();

            Dictionary<string, double> tokenProbs = new Dictionary<string, double>();
            List<ClassifyData> classifyDatas = new List<ClassifyData>();


            double sum = double.Epsilon;

            // Составление словаря и расчет суммы
            for (int i = 0; i < topExp.Count; i++)
            {
                string token = topExp[i].Token;
                double prob = topExp[i].Logprob;

                if (TokenValues.Contains(token))
                {
                    if (!tokenProbs.ContainsKey(token))
                        tokenProbs.Add(token, prob);
                    else 
                        tokenProbs[token] += prob;

                    sum += topExp[i].Logprob; 
                }
            }

            // Рассчет вероятностей
            foreach(var tokenProb in tokenProbs)
                classifyDatas.Add(new ClassifyData() 
                { ClToken = tokenProb.Key, Prob = tokenProb.Value / sum });

            return classifyDatas;
        }

    }

    /// <summary>
    /// Данные для классификатора
    /// </summary>
    public class ClassifyData 
    {
        /// <summary>
        /// Имя токена
        /// </summary>
        public string ClToken { get; set; }

        /// <summary>
        /// Вероятность
        /// </summary>
        public double Prob { get; set; }
    }
}
