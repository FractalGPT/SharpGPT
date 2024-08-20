using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FractalGPT.SharpGPTLib.API.FractalGPTAPI.QA
{
    /// <summary>
    /// A client for interacting with the FractalGPT QA API.
    /// </summary>
    [Serializable]
    public class FractalGPTQAClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        /// <summary>
        /// Initializes a new instance of the FractalGPTClient class with the specified API key.
        /// </summary>
        /// <param name="apiKey">The API key for authenticating with the FractalGPT service.</param>
        public FractalGPTQAClient(string apiKey)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://fractalgpt.ru/api/qa/")
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        /// <summary>
        /// Asynchronously uploads a file to the FractalGPT service.
        /// </summary>
        /// <param name="indexName">The name of the index to upload the document to.</param>
        /// <param name="documentType">The type of the document being uploaded.</param>
        /// <param name="documentName">The name of the document being uploaded.</param>
        /// <param name="filePath">The path to the file to upload.</param>
        /// <returns>A task representing the asynchronous operation, containing the HTTP response.</returns>
        public async Task<HttpResponseMessage> UploadFileAsync(string indexName, string documentType, string documentName, string filePath)
        {
            var url = "upload";
            byte[] fileBytes;

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fileBytes = new byte[fs.Length];
                _ = await fs.ReadAsync(fileBytes, 0, fileBytes.Length);
            }

            var payload = new
            {
                index_name = indexName,
                document_type = documentType,
                document_name = documentName,
                data = fileBytes
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            return await _httpClient.PostAsync(url, content);
        }

        /// <summary>
        /// Asynchronously retrieves an answer from the FractalGPT service for a given question.
        /// </summary>
        /// <param name="indexName">The name of the index to query.</param>
        /// <param name="question">The question to get an answer for.</param>
        /// <returns>A task representing the asynchronous operation, containing the HTTP response.</returns>
        public async Task<HttpResponseMessage> GetAnswerAsync(string indexName, string question)
        {
            var url = "get-answer";
            var payload = new
            {
                index_name = indexName,
                question
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            return await _httpClient.PostAsync(url, content);
        }

        /// <summary>
        /// Asynchronously deletes an index from the FractalGPT service.
        /// </summary>
        /// <param name="indexName">The name of the index to delete.</param>
        /// <returns>A task representing the asynchronous operation, containing the HTTP response.</returns>
        public async Task<HttpResponseMessage> DeleteIndexAsync(string indexName)
        {
            var url = $"delete?index_name={indexName}";
            return await _httpClient.DeleteAsync(url);
        }
    }

}
