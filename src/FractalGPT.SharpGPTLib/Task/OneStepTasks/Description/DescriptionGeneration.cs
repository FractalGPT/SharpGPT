using FractalGPT.SharpGPTLib.API;
using FractalGPT.SharpGPTLib.Prompts;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace FractalGPT.SharpGPTLib.Task.OneStepTasks.Description
{
    /// <summary>
    /// Service for generating descriptions using Large Language Models (LLMs).
    /// </summary>
    [Serializable]
    public class DescriptionGeneration
    {
        private readonly IText2TextAPI _textToTextApi;
        private readonly string _lang;

        /// <summary>
        /// Constructor for creating a new instance of the description generation service.
        /// </summary>
        /// <param name="text2TextApi">API for text-to-text transformation.</param>
        /// <param name="lang">The language for generating the description (default is English).</param>
        public DescriptionGeneration(IText2TextAPI text2TextApi, string lang = "en")
        {
            _lang = lang;
            _textToTextApi = text2TextApi ?? throw new ArgumentNullException(nameof(text2TextApi));
            _textToTextApi.SetPrompt(PromptManager.SystemPrompts["description_generation_en"]); // It is assumed to be a constant or a static resource.
        }

        /// <summary>
        /// Asynchronously generates a description for the provided text.
        /// </summary>
        /// <param name="text">The text for which the description needs to be generated.</param>
        /// <returns>The generated description of the original text.</returns>
        public async Task<string> SummarizeAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text cannot be null or whitespace.", nameof(text));
            }

            string prompt = TaskPrompts.InputPrompt(text, "description_generation", _lang);
            string summary = await _textToTextApi.SendAsyncReturnText(prompt);
            return summary;
        }

        /// <summary>
        /// Synchronously generates a description for the provided text.
        /// </summary>
        /// <param name="text">The text for which the description needs to be generated.</param>
        /// <returns>The generated description of the original text.</returns>
        public string Summarize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text cannot be null or whitespace.", nameof(text));
            }

            string prompt = TaskPrompts.InputPrompt(text, "description_generation", _lang);
            string summary = _textToTextApi.SendReturnText(prompt);
            return summary;
        }
    }
}