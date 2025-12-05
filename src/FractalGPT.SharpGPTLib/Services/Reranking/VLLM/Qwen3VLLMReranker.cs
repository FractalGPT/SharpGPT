using FractalGPT.SharpGPTLib.Core.Models.Providers.VLLM;
using FractalGPT.SharpGPTLib.Infrastructure.Extensions;
using System.Net.Http.Json;

namespace FractalGPT.SharpGPTLib.Services.Reranking.VLLM;

public class Qwen3VLLMReranker
{
    public const string Prefix = "<|im_start|>system\nJudge whether the Document meets the requirements based on the Query and the Instruct provided. Note that the answer can only be \"yes\" or \"no\".<|im_end|>\n<|im_start|>user\n";
    public const string Suffix = "<|im_end|>\n<|im_start|>assistant\n<think>\n\n</think>\n\n";
    //public const string BaseInstruction = "Given a web search query, retrieve relevant passages that answer the query";
    public const string BaseInstruction = "Ты - интеллектуальная поисковая система для поиска релевантных документов по их описанию. Укажи набилее релевантные документы к запросу";
    public const string ProductInstruction = "Ты - интеллектуальная поисковая система для поиска товаров по их описанию. Укажи наиболее релевантные товары к запросу";
    public const string QueryTemplate = "{Prefix}<Instruct>: {Instruction}\n<Query>: {Query}\n";
    public const string DocumentTemplate = "<Document>: {Document}{Suffix}";

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
    /// Инициализирует новый экземпляр класса <see cref="Qwen3VLLMReranker"/>
    /// </summary>
    /// <param name="apiUrl">Базовый URL API</param>
    /// <param name="rerankerModelName"> Имя модели реранкера</param>
    public Qwen3VLLMReranker(string apiUrl, string rerankerModelName)
    {
        _apiUrl = apiUrl;
        RerankerModelName = rerankerModelName;

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_apiUrl),
            Timeout = TimeSpan.FromMinutes(5)
        };
    }

    /// <summary>
    /// Отправляет запрос на ранжирование документов
    /// </summary>
    /// <param name="query">Запрос, относительно которого ранжируются документы</param>
    /// <param name="documents">Список документов для ранжирования</param>
    /// <param name="instruct">Инструкция</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Ответ от сервера с результатами ранжирования</returns>
    public async Task<VLLMRerankResponse> RerankAsync(string query, IEnumerable<string> documents, string instruct = null, CancellationToken cancellationToken = default)
    {
        var queryPrompt = QueryTemplate
            .Replace("{Prefix}", Prefix)
            .Replace("{Instruction}", instruct ?? BaseInstruction)
            .Replace("{Query}", query);

        var documentPrompts = documents
            .Select(doc => DocumentTemplate.Replace("{Suffix}", Suffix)
                                           .Replace("{Document}", doc))
            .ToArray();

        Exception lastException = new();
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {

                using var response = await _httpClient.PostAsJsonAsync("/rerank", new VLLMRerankRequest
                {
                    Model = RerankerModelName,
                    Query = queryPrompt,
                    Documents = documentPrompts
                }, linkedCts.Token);

                if (!response.IsSuccessStatusCode)
                    throw new Exception((await response.Content.ReadAsStringAsync(linkedCts.Token) ?? "").TruncateForLogging());

                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<VLLMRerankResponse>(cancellationToken: linkedCts.Token);
                result.Results = [.. result.Results.OrderBy(t => t.Index)];
                return result;
            }
            catch (Exception ex)
            {
                lastException = ex;
                if (attempt < 1) // Только для первой попытки
                {
                    try { await Task.Delay(1000, linkedCts.Token); }
                    catch (OperationCanceledException) { throw lastException; }
                }
            }
        }

        throw lastException;
    }
}
