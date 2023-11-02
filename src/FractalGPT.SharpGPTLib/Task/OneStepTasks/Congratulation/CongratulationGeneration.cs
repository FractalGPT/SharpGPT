using FractalGPT.SharpGPTLib.API;
using FractalGPT.SharpGPTLib.Prompts;
using System;

namespace FractalGPT.SharpGPTLib.Task.OneStepTasks.Congratulation
{
    /// <summary>
    /// Represents a specialized chat model focused on generating congratulatory messages.
    /// </summary>
    [Serializable]
    public class CongratulationGeneration : SimplePromptTaskChatModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CongratulationGeneration"/> class with a predefined prompt for generating congratulatory messages.
        /// </summary>
        /// <param name="text2Text">The text-to-text API implementation to be used for generating congratulatory messages.</param>
        public CongratulationGeneration(IText2TextAPI text2Text)
            : base(text2Text, PromptManager.SystemPrompts["congratulation_generation_en"])
        {
            // The constructor is intentionally left blank.
        }
    }
}
