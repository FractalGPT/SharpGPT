using FractalGPT.SharpGPTLib.API;
using FractalGPT.SharpGPTLib.Prompts;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace FractalGPT.SharpGPTLib.Task.Summarizing
{
    /// <summary>
    /// Text summarization service using Large Language Models (LLMs).
    /// </summary>
    [Serializable]
    public class SummarizationService
    {
        private readonly IText2TextChat _textToTextApi;
        private readonly string _lang;

        /// <summary>
        /// Creates a new instance of the summarization service.
        /// </summary>
        /// <param name="text2TextApi">API for text-to-text transformation.</param>
        public SummarizationService(IText2TextChat text2TextApi, string lang = "en")
        {
            _lang = lang;
            _textToTextApi = text2TextApi ?? throw new ArgumentNullException(nameof(text2TextApi));
            _textToTextApi.SetPrompt(PromptsChatGPT.ChatGPTSummarizationPromptEN); // It's assumed that this is a constant or a static resource.
        }

        /// <summary>
        /// Asynchronously summarizes the provided text.
        /// </summary>
        /// <param name="text">The text to be summarized.</param>
        /// <returns>The summarized version of the original text.</returns>
        public async Task<string> SummarizeAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text cannot be null or whitespace.", nameof(text));
            }

            // RuPrompts is a static class with resources.
            string prompt = TaskPrompts.InputPrompt(text, "summarization", _lang);
            string summary = await _textToTextApi.SendAsyncReturnText(prompt);
            return summary;
        }

        /// <summary>
        /// Synchronously summarizes the provided text.
        /// </summary>
        /// <param name="text">The text to be summarized.</param>
        /// <returns>The summarized version of the original text.</returns>
        public string Summarize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text cannot be null or whitespace.", nameof(text));
            }

            // RuPrompts is a static class with resources.
            string prompt = TaskPrompts.InputPrompt(text, "summarization", _lang);
            string summary = _textToTextApi.SendReturnText(prompt);
            return summary;
        }
    }
}