using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace FractalGPT.SharpGPTLib.API.AI21
{
    /// <summary>
    /// A class to interact with the AI21 Studio API.
    /// </summary>
    [Serializable]
    public class AI21StudioAPI : IText2Text
    {
        private readonly AuthenticationHeaderValue _header;
        private readonly string _modelPath;
        private static readonly HttpClient client = new HttpClient();
        private AI21GenerationParameters _generationParametrs;

        /// <summary>
        /// Initializes a new instance of the AI21StudioAPI class.
        /// </summary>
        /// <param name="key">The API key for authentication.</param>
        /// <param name="modelPath">The path to the AI model.</param>
        public AI21StudioAPI(string key, string model = "j2-grande-instruct")
        {
            string modelPath = $"https://api.ai21.com/studio/v1/{model}/complete";
            string newKey = key ?? throw new ArgumentNullException(nameof(key));
            _header = new AuthenticationHeaderValue("Bearer", newKey);
            _modelPath = modelPath ?? throw new ArgumentNullException(nameof(modelPath));
            SetParams();
        }

        /// <summary>
        /// Asynchronously sends a query to the AI21 Studio API.
        /// </summary>
        /// <param name="text">The input text for the AI model.</param>
        /// <returns>The response from the AI model.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API request is unsuccessful.</exception>
        public async Task<string> SendAsyncReturnText(string text)
        {
            var content = _generationParametrs.ToStringContent(text);
            client.DefaultRequestHeaders.Authorization = _header;

            var response = await client.PostAsync(_modelPath, content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<ResponseData>(responseString);
                return responseData.Completions[0].Data.Text;
            }
            else
            {
                throw new HttpRequestException($"Error in API request: {(int)response.StatusCode} {response.ReasonPhrase}");
            }
        }

        /// <summary>
        /// Synchronously sends a query to the AI21 Studio API.
        /// </summary>
        /// <param name="text">The input text for the AI model.</param>
        /// <param name="numResults">The number of results to return.</param>
        /// <param name="maxTokens">The maximum number of tokens to generate.</param>
        /// <param name="stopSequences">The sequences where the generation should stop.</param>
        /// <param name="temperature">The creativity temperature.</param>
        /// <param name="topKReturn">The number of top results to consider.</param>
        /// <param name="presencePenaltyScale">The scale of presence penalty.</param>
        /// <returns>The response from the AI model.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API request is unsuccessful.</exception>
        public string SendReturnText(string text)
        {
            var content = _generationParametrs.ToStringContent(text);
            client.DefaultRequestHeaders.Authorization = _header;

            var response = client.PostAsync(_modelPath, content).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseString = response.Content.ReadAsStringAsync().Result;
                var responseData = JsonSerializer.Deserialize<ResponseData>(responseString);
                return responseData.Completions[0].Data.Text;
            }
            else
            {
                throw new HttpRequestException($"Error in API request: {(int)response.StatusCode} {response.ReasonPhrase}");
            }
        }

        /// <summary>
        /// Set Parametrs
        /// </summary>
        /// <param name="numResults">The number of results to return.</param>
        /// <param name="maxTokens">The maximum number of tokens to generate.</param>
        /// <param name="stopSequences">The sequences where the generation should stop.</param>
        /// <param name="temperature">The creativity temperature.</param>
        /// <param name="topKReturn">The number of top results to consider.</param>
        /// <param name="presencePenaltyScale">The scale of presence penalty.</param>
        public void SetParams(int numResults = 1, int maxTokens = 80, string[] stopSequences = null, double temperature = 0.6, int topKReturn = 4, double presencePenaltyScale = 1.1)
        {
            _generationParametrs = new AI21GenerationParameters(numResults, maxTokens, stopSequences, temperature, topKReturn, presencePenaltyScale);
        }
    }

}
