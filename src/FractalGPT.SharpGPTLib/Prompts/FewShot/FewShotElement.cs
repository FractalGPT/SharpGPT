﻿namespace FractalGPT.SharpGPTLib.Prompts.FewShot
{

    /// <summary>
    /// Represents an element for few-shot learning scenarios, encapsulating a prompt and its corresponding model output.
    /// </summary>
    [Serializable]
    public class FewShotElement
    {
        /// <summary>
        /// Gets or sets the prompt used for generating the model output.
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// Gets or sets the output generated by the model in response to the prompt.
        /// </summary>
        public string ModelOutput { get; set; }

        /// <summary>
        /// Initializes a new instance of the FewShotElement class.
        /// </summary>
        public FewShotElement() { }

        /// <summary>
        /// Initializes a new instance of the FewShotElement class.
        /// </summary>
        public FewShotElement(string prompt, string output)
        {
            Prompt = prompt;
            ModelOutput = output;
        }

        /// <summary>
        /// Constructs a formatted string that combines the prompt and model output with specified start and end tokens, and a separator.
        /// </summary>
        /// <param name="startToken">The start token to prepend to the formatted string. Default is "&lt;s&gt;".</param>
        /// <param name="endToken">The end token to append to the formatted string. Default is "&lt;/s&gt;".</param>
        /// <param name="sep">The separator to use between the prompt and model output. Default is a newline character.</param>
        /// <returns>A formatted string that combines the prompt and model output.</returns>
        public virtual string GetString(string startToken = "<s>", string endToken = "</s>", string sep = "\n")
        {
            return $"{startToken}{Prompt}{sep}{ModelOutput}{endToken}";
        }

        /// <summary>
        /// Returns a string that represents the current object, specifically formatting the prompt and model output for readability.
        /// </summary>
        /// <returns>A string representation of the FewShotElement instance, showing the prompt and model output separated by two newlines.</returns>
        public override string ToString()
        {
            return $"prompt = {Prompt}\n\noutput = {ModelOutput}";
        }
    }
}
