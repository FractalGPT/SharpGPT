using FractalGPT.SharpGPTLib.API;
using FractalGPT.SharpGPTLib.Prompts;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FractalGPT.SharpGPTLib.Task.DialogTasks
{
    /// <summary>
    /// Represents a dialog system that interacts with a text-to-text API to send and receive text.
    /// </summary>
    [Serializable]
    public class TextDialog
    {
        private readonly IText2TextChatAPI _text2Text; // This field is readonly because it is only set in the constructor
        private readonly string _lang;
        private string _text;
        bool text_loaded = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextDialog"/> class.
        /// </summary>
        /// <param name="text2Text">The text-to-text API implementation.</param>
        /// <param name="lang">The language setting for the dialog (default is "en").</param>
        public TextDialog(IText2TextChatAPI text2Text, string lang = "en")
        {
            _lang = lang;
            _text2Text = text2Text ?? throw new ArgumentNullException(nameof(text2Text));
            string prompt = $"{PromptManager.SystemPrompts["text_dialog_en"]}";
            _text2Text.SetPrompt(prompt);
        }

        /// <summary>
        /// Sets the initial text prompt for the dialog.
        /// </summary>
        /// <param name="text">The text to set as the initial prompt.</param>
        public void LoadText(string text)
        {
            _text = text;
        }

        /// <summary>
        /// Asynchronously generates a response for the given input.
        /// </summary>
        /// <param name="input">The input text for which to generate a response.</param>
        /// <returns>A task that represents the asynchronous operation, containing the generated response.</returns>
        public async Task<string> GenerateAsync(string input)
        {
            string ans = await _text2Text.SendAsyncReturnText(get_prompt(input));
            return ans;
        }

        /// <summary>
        /// Synchronously generates a response for the given input.
        /// </summary>
        /// <param name="input">The input text for which to generate a response.</param>
        /// <returns>The generated response.</returns>
        public string Generate(string input)
        {
            string ans = _text2Text.SendReturnText(get_prompt(input));
            return ans;
        }

        /// <summary>
        /// Clears the context or history of the dialog.
        /// </summary>
        public void Clear()
        {
            _text2Text.ClearContext();
        }


        private string get_prompt(string input) 
        {
            string q = _lang == "ru"? "Вопрос": "Question";

            string text_q = text_loaded ? input : $"Text {_text}\n\n{q}: {input}";
            text_loaded = true;
            return text_q;
        }
    }
}
