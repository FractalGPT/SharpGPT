namespace FractalGPT.SharpGPTLib.API.AI21
{
    /// <summary>
    /// Represents a single completion with its associated data.
    /// </summary>
    [Serializable]
    public class Completion
    {
        /// <summary>
        /// Gets or sets the data associated with this completion, usually containing the text.
        /// </summary>
        public CompletionData Data { get; set; }
    }

}
