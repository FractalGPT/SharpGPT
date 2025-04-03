using System.Net.Http.Json;

namespace FractalGPT.SharpGPTLib.Encoders.Reranker.Infinity;

/// <summary>
/// Класс для работы с API ранжирования документов
/// </summary>
public class InfinityReranker
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Имя модели реранкера
    /// </summary>
    public string RerankerModelName { get; set; }

    /// <summary>
    /// Базовый URL API
    /// </summary>
    private readonly string _apiUrl;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="InfinityReranker"/>
    /// </summary>
    /// <param name="apiUrl">Базовый URL API</param>
    /// <param name="rerankerModelName"> Имя модели реранкера</param>
    public InfinityReranker(string apiUrl, string rerankerModelName)
    {
        _apiUrl = apiUrl;
        RerankerModelName = rerankerModelName;

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_apiUrl)
        };
    }

    /// <summary>
    /// Отправляет запрос на ранжирование документов
    /// </summary>
    /// <param name="query">Запрос, относительно которого ранжируются документы</param>
    /// <param name="documents">Список документов для ранжирования</param>
    /// <returns>Ответ от сервера с результатами ранжирования</returns>
    public async Task<RerankResponse> RerankAsync(string query, List<string> documents)
    {
        Exception lastException = new Exception();

        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {

                using var response = await _httpClient.PostAsJsonAsync("/v1/rerank", new RerankRequest
                {
                    Model = RerankerModelName,
                    Query = query,
                    Documents = documents
                });

                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<RerankResponse>();
                result.Results = result.Results.OrderBy(t => t.Index).ToList();
                return result;
            }
            catch (Exception ex)
            {
                lastException = ex;
                await Task.Delay(1000);
            }
        }

        throw lastException;
    }
}
