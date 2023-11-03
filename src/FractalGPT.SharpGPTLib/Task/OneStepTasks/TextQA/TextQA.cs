using FractalGPT.SharpGPTLib.API;
using FractalGPT.SharpGPTLib.Prompts;
using System;
using System.Numerics;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FractalGPT.SharpGPTLib.Task.TextQA
{
    /// <summary>
    /// Class for handling questions and answers based on a given text.
    /// </summary>
    [Serializable]
    public class TextQA
    {
        private readonly IText2TextChatAPI _text2Text;
        private string _text;
        private readonly string _lang;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextQA"/> class with the given text API.
        /// </summary>
        /// <param name="text2Text">API for text-to-text transformation.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text2Text"/> is null.</exception>
        public TextQA(IText2TextChatAPI text2Text, string lang)
        {
            _lang = lang;
            _text2Text = text2Text ?? throw new ArgumentNullException(nameof(text2Text));
            _text2Text.SetPrompt(PromptsChatGPT.ChatGPTTextQAPromptEN); // It's assumed that this method is safe and won't throw an exception.
        }

        /// <summary>
        /// Loads the text for subsequent QA operations.
        /// </summary>
        /// <param name="text">The text based on which answers to questions will be formed.</param>
        /// <exception cref="ArgumentException">Thrown if the text is empty or consists only of whitespace.</exception>
        public void LoadText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Provided text cannot be null or whitespace.", nameof(text));
            }

            _text = text;
        }

        /// <summary>
        /// Asynchronously gets an answer to a given question using the loaded text.
        /// </summary>
        /// <param name="question">The question to get an answer for.</param>
        /// <returns>The answer to the given question.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the text was not loaded before calling this method.</exception>
        /// <exception cref="ArgumentException">Thrown if the question is empty or consists only of whitespace.</exception>
        public async Task<string> GetAnswerAsync(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                throw new ArgumentException("Question cannot be null or whitespace.", nameof(question));
            }

            if (_text == null)
            {
                throw new InvalidOperationException("Text must be loaded before getting answers.");
            }

            string input = GetPrompt(question); // This assumes that an external method formats the requests.
            return await _text2Text.SendAsyncReturnText(input);
        }

        /// <summary>
        /// Gets an answer to a given question using the loaded text.
        /// </summary>
        /// <param name="question">The question to get an answer for.</param>
        /// <returns>The answer to the given question.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the text was not loaded before calling this method.</exception>
        /// <exception cref="ArgumentException">Thrown if the question is empty or consists only of whitespace.</exception>
        public string GetAnswer(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                throw new ArgumentException("Question cannot be null or whitespace.", nameof(question));
            }

            if (_text == null)
            {
                throw new InvalidOperationException("Text must be loaded before getting answers.");
            }

            string input = GetPrompt(question); // This assumes that an external method formats the requests.
            return _text2Text.SendReturnText(input);
        }


        private string GetPrompt(string q)
        {
            return TaskPrompts.InputPrompt(_text, "text_qa", _lang) + q;
        }
    }
}
