using AI.DataStructs.Algebraic;
using AI.Statistics;
using FractalGPT.SharpGPTLib.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FractalGPT.SharpGPTLib.LLM
{
    public class SuperChatLLM : IText2TextChat
    {
        private readonly Random _random = new Random();

        /// <summary>
        /// A neural network working on the principle of a chatbot,
        /// and a function for assessing its quality (heuristics)
        /// </summary>
        public List<(IText2TextChat, Func<string, double>)> LLMs { get; set; }
            = new List<(IText2TextChat, Func<string, double>)>();

        public void ClearContext()
        {
            foreach (var item in LLMs)
                item.Item1.ClearContext();
        }

        public Task<string> SendReturnTextAsync(string text)
        {
            return SelectionLLM(text).SendReturnTextAsync(text);
        }

        public string SendReturnText(string text)
        {
            return SelectionLLM(text).SendReturnText(text);
        }

        public void SetPrompt(string prompt)
        {
            foreach (var item in LLMs)
                item.Item1.SetPrompt(prompt);
        }

        // Выбор сетки
        public IText2TextChat SelectionLLM(string text)
        {
            IText2TextChat[] llms = new IText2TextChat[LLMs.Count];
            Vector qs = new Vector(LLMs.Count);

            for (int i = 0; i < llms.Length; i++)
            {
                llms[i] = LLMs[i].Item1;
                qs[i] = LLMs[i].Item2(text);
            }

            qs = qs.Minimax();
            qs /= qs.Sum();

            return RandomItemSelection.GetElement(qs, llms, _random);
        }
    }
}
