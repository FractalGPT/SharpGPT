using FractalGPT.SharpGPTLib.API;
using FractalGPT.SharpGPTLib.Prompts;
using System;

namespace FractalGPT.SharpGPTLib.Task.OneStepTasks.Letter
{
    /// <summary>
    /// Represents a specialized chat model focused on letter generation tasks.
    /// </summary>
    [Serializable]
    public class LetterGeneration : SimplePromptTaskChatModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LetterGeneration"/> class with a predefined prompt for letter generation.
        /// </summary>
        /// <param name="text2Text">The text-to-text API implementation to be used for letter generation.</param>
        public LetterGeneration(IText2TextAPI text2Text)
            : base(text2Text, PromptManager.SystemPrompts["letter_generation_en"])
        {
            // The constructor is intentionally left blank.
        }
    }
}
