using FractalGPT.SharpGPTLib.API;
using FractalGPT.SharpGPTLib.Prompts;

namespace FractalGPT.SharpGPTLib.Tasks.OneStepTasks.Code
{
    /// <summary>
    /// Represents a specialized chat model focused on code generation tasks.
    /// </summary>
    [Serializable]
    public class CodeGeneration : SimplePromptTaskChatModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeGeneration"/> class with a predefined prompt for code generation.
        /// </summary>
        /// <param name="text2Text">The text-to-text API implementation to be used for code generation.</param>
        public CodeGeneration(IText2TextChat text2Text)
            : base(text2Text, PromptManager.SystemPrompts["code_generation_en"])
        {
            // The constructor is intentionally left blank.
        }
    }
}
