using System;

namespace FractalGPT.SharpGPTLib.API.LocalServer
{
    /// <summary>
    /// Represents the parameters for generation processes.
    /// </summary>
    [Serializable]
    public class GenerationParametrs
    {
        /// <summary>
        /// Gets or sets the maximum length of the generated content.
        /// </summary>
        public int MaxLen { get; set; } = 50;

        /// <summary>
        /// Gets or sets the probability threshold for top-p sampling.
        /// </summary>
        public double TopP { get; set; } = 0.8;

        /// <summary>
        /// Gets or sets the number of top tokens to consider for top-k sampling.
        /// </summary>
        public int TopK { get; set; } = 15;

        /// <summary>
        /// Gets or sets the temperature for sampling, affecting randomness.
        /// </summary>
        public double Temperature { get; set; } = 0.6;

        /// <summary>
        /// Gets or sets the size of the n-gram to prevent repeating sequences.
        /// </summary>
        public int NoRepeatNgramSize { get; set; } = 3;

        /// <summary>
        /// Initializes a new instance of the GenerationParametrs class.
        /// </summary>
        public GenerationParametrs() { }

        /// <summary>
        /// Initializes a new instance of the GenerationParametrs class with specified parameters.
        /// </summary>
        /// <param name="maxLen">The maximum length of the generated content.</param>
        /// <param name="topP">The probability threshold for top-p sampling.</param>
        /// <param name="topK">The number of top tokens to consider for top-k sampling.</param>
        /// <param name="temperature">The temperature for sampling.</param>
        /// <param name="noRepeatNgramSize">The size of the n-gram to prevent repeating sequences.</param>
        public GenerationParametrs(int maxLen, double topP, int topK, double temperature, int noRepeatNgramSize)
        {
            MaxLen = maxLen;
            TopP = topP;
            TopK = topK;
            Temperature = temperature;
            NoRepeatNgramSize = noRepeatNgramSize;
        }
    }

}
