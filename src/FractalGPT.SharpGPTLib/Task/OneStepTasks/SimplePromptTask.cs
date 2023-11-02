using FractalGPT.SharpGPTLib.API;
using System;
using System.Threading.Tasks;

namespace FractalGPT.SharpGPTLib.Task.OneStepTasks
{
    /// <summary>
    /// Represents a simple chat model that sends and receives text using the IText2TextAPI interface.
    /// </summary>
    [Serializable]
    public class SimplePromptTaskChatModel
    {
        private readonly IText2TextAPI _text2Text; // Use readonly as this field is only set in constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplePromptTaskChatModel"/> class.
        /// </summary>
        /// <param name="text2Text">The text-to-text API implementation.</param>
        /// <param name="prompt">The initial prompt to set for the chat model.</param>
        public SimplePromptTaskChatModel(IText2TextAPI text2Text, string prompt)
        {
            _text2Text = text2Text ?? throw new ArgumentNullException(nameof(text2Text));
            _text2Text.SetPrompt(prompt);
        }

        /// <summary>
        /// Asynchronously generates a response for the given input.
        /// </summary>
        /// <param name="input">The input text for which to generate a response.</param>
        /// <returns>A task that represents the asynchronous operation, containing the generated response.</returns>
        public async Task<string> GenerateAsync(string input)
        {
            string ans = await _text2Text.SendAsyncReturnText(input);
            _text2Text.ClearContext(); // Consider whether you really want to clear context after each message
            return ans;
        }

        /// <summary>
        /// Synchronously generates a response for the given input.
        /// </summary>
        /// <param name="input">The input text for which to generate a response.</param>
        /// <returns>The generated response.</returns>
        public string Generate(string input)
        {
            string ans = _text2Text.SendReturnText(input);
            _text2Text.ClearContext(); // Consider whether you really want to clear context after each message
            return ans;
        }
    }
}

