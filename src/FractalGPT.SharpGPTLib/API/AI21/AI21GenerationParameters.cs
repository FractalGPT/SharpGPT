using System.Text.Json;

namespace FractalGPT.SharpGPTLib.API.AI21
{
    /// <summary>
    /// Represents the parameters for AI21 Studio API text generation.
    /// </summary>
    [Serializable]
    public class AI21GenerationParameters
    {
        /// <summary>
        /// Gets or sets the number of results to return.
        /// </summary>
        public int NumResults { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of tokens to generate.
        /// </summary>
        public int MaxTokens { get; set; }

        /// <summary>
        /// Gets or sets the sequences where the generation should stop.
        /// </summary>
        public IEnumerable<string> StopSequences { get; set; }

        /// <summary>
        /// Gets or sets the creativity temperature.
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        /// Gets or sets the number of top results to consider.
        /// </summary>
        public int TopKReturn { get; set; }

        /// <summary>
        /// Gets or sets the scale of presence penalty.
        /// </summary>
        public double PresencePenaltyScale { get; set; }

        /// <summary>
        /// Initializes a new instance of the AI21GenerationParameters class.
        /// </summary>
        public AI21GenerationParameters() { }

        /// <summary>
        /// Initializes a new instance of the AI21GenerationParameters class with specified parameters.
        /// </summary>
        /// <param name="numResults">The number of results to return.</param>
        /// <param name="maxTokens">The maximum number of tokens to generate.</param>
        /// <param name="stopSequences">The sequences where the generation should stop.</param>
        /// <param name="temperature">The creativity temperature.</param>
        /// <param name="topKReturn">The number of top results to consider.</param>
        /// <param name="presencePenaltyScale">The scale of presence penalty.</param>
        public AI21GenerationParameters(int numResults = 1, int maxTokens = 80, IEnumerable<string> stopSequences = null, double temperature = 0.6, int topKReturn = 4, double presencePenaltyScale = 1.1)
        {
            NumResults = numResults;
            MaxTokens = maxTokens;
            StopSequences = stopSequences;
            Temperature = temperature;
            TopKReturn = topKReturn;
            PresencePenaltyScale = presencePenaltyScale;
        }

        /// <summary>
        /// Converts the parameters to a JSON payload for the AI21 Studio API request.
        /// </summary>
        /// <param name="text">The input text for the AI model.</param>
        /// <returns>A StringContent representing the JSON payload.</returns>
        public StringContent ToStringContent(string text)
        {
            var jsonPayload = new
            {
                prompt = text,
                NumResults,
                MaxTokens,
                StopSequences,
                Temperature,
                TopKReturn,
                presencePenalty = new { scale = PresencePenaltyScale },
            };

            var content = new StringContent(JsonSerializer.Serialize(jsonPayload), System.Text.Encoding.UTF8, "application/json");
            return content;
        }
    }
}
