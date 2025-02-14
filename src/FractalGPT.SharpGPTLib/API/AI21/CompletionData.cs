namespace FractalGPT.SharpGPTLib.API.AI21
{
    /// <summary>
    /// Represents the data associated with a completion, typically the text output.
    /// </summary>
    [Serializable]
    public class CompletionData
    {
        /// <summary>
        /// Gets or sets the text of the completion.
        /// </summary>
        public string Text { get; set; }
    }

}
