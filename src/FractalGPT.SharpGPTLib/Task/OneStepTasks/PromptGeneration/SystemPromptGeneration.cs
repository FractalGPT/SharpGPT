using FractalGPT.SharpGPTLib.API;
using FractalGPT.SharpGPTLib.Prompts;

namespace FractalGPT.SharpGPTLib.Tasks.PromptGeneration
{
    /// <summary>
    /// Prompt generator
    /// </summary>
    [Serializable]
    public class SystemPromptGeneration
    {
        private readonly IText2TextChat _text2Text;
        private readonly string _lang;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemPromptGeneration"/> class with the given text API.
        /// </summary>
        /// <param name="text2Text">API for text-to-text transformation.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text2Text"/> is null.</exception>
        public SystemPromptGeneration(IText2TextChat text2Text, string lang)
        {
            _lang = lang;
            _text2Text = text2Text ?? throw new ArgumentNullException(nameof(text2Text));
            // Initialize with a default system prompt.
            _text2Text.SetPrompt(PromptsChatGPT.ChatGPTPromptGenerationPromptEN);
        }

        /// <summary>
        /// Asynchronously generates a system prompt based on the input.
        /// </summary>
        /// <param name="input">The input for which a system prompt is needed.</param>
        /// <returns>The generated system prompt.</returns>
        /// <exception cref="ArgumentException">Thrown if the input is empty or consists only of whitespace.</exception>
        public async Task<string> GeneratePromptAsync(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input cannot be null or whitespace.", nameof(input));
            }

            string formattedInput = GetFormattedPrompt(input);
            return await _text2Text.SendReturnTextAsync(formattedInput);
        }

        /// <summary>
        /// Generates a system prompt based on the input.
        /// </summary>
        /// <param name="input">The input for which a system prompt is needed.</param>
        /// <returns>The generated system prompt.</returns>
        /// <exception cref="ArgumentException">Thrown if the input is empty or consists only of whitespace.</exception>
        public string GeneratePrompt(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input cannot be null or whitespace.", nameof(input));
            }

            string formattedInput = GetFormattedPrompt(input);
            return _text2Text.SendReturnText(formattedInput);
        }

        /// <summary>
        /// Formats the input to be sent for prompt generation.
        /// </summary>
        /// <param name="input">The input to format.</param>
        /// <returns>The formatted input.</returns>
        private string GetFormattedPrompt(string task)
        {
            var prompt_dict = PromptManager.SystemPrompts;
            string promts = "";
            string taskD = _lang == "en" ? "Task name" : "Имя задачи";
            string promptText = _lang == "en" ? "Prompt" : "Подсказка";

            foreach (var prompt in prompt_dict)
                promts += $"{taskD}: {prompt.Key}\t{promptText}: {prompt.Value}\n";

            return TaskPrompts.InputPrompt(promts, "system_prompt_generation", _lang) + task;
        }
    }
}