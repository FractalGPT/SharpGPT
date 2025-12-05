using AI.DataStructs.Algebraic;
using FractalGPT.SharpGPTLib.Core.Abstractions;
using FractalGPT.SharpGPTLib.Infrastructure.Extensions;
using FractalGPT.SharpGPTLib.Services.Embeddings.Infinity.Models;
using System.Net.Http.Json;

namespace FractalGPT.SharpGPTLib.Services.Embeddings.Infinity;

public class BaseInfinityEmbedder : IEmbedderService
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Параметры тангенса "k", f(x) = tanh(k*x+b) 
    /// </summary>
    public double TanhNormParamK { get; set; } = 0.64;

    /// <summary>
    /// Параметры тангенса "b", f(x) = tanh(k*x+b) 
    /// </summary>
    public double TanhNormParamB { get; set; } = 0.55;
    /// <summary>
    /// СКО косинуса
    /// </summary>
    public double StdCos { get; set; } = 1;
    /// <summary>
    /// Среднее косинуса
    /// </summary>
    public double MeanCos { get; set; } = 1;

    public virtual string ModelName { get; set; }

    public virtual string GetDetailedInstruct(string question) => throw new NotImplementedException();

    /// <summary>
    /// The base URL of the local server.
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseAPI"/> class with an optional host URL.
    /// </summary>
    /// <param name="host">The base URL of the local server.</param>
    public BaseInfinityEmbedder(string host = "http://172.17.0.1:11111/")
    {
        Host = host;
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(host),
            Timeout = TimeSpan.FromMinutes(5)
        };
    }

    public Task<Vector[]> EncodeAsync(IEnumerable<string> texts, CancellationToken cancellationToken = default) =>
        EncodeBaseAsync(texts, cancellationToken);


    public Task<Vector> EncodeAsync(string text, CancellationToken cancellationToken = default) =>
        EncodeBaseAsync(text, cancellationToken);

    public Task<Vector> EncodeQuestionAsync(string text, CancellationToken cancellationToken = default) =>
        EncodeQuestionBaseAsync(text, cancellationToken);

    /// <summary>
    /// Нормализация косинуса через гиперболический тангенс
    /// </summary>
    public virtual double TanhCosineNormalize(double cosine) =>
        Math.Tanh(TanhNormParamK * (cosine - MeanCos) / StdCos + TanhNormParamB);

    public async Task<Vector[]> EncodeAsyncWithBlockSize(IEnumerable<string> processedTexts, IEnumerable<int> blockSizes, IEnumerable<int> excludeBlockSizes = null, CancellationToken cancellationToken = default)
    {
        var snippetsTexts = processedTexts.ToArray();
        var blockSizesArray = blockSizes.ToArray();

        if (snippetsTexts.Length != blockSizesArray.Length)
            throw new Exception("Array size mismatch");

        List<int> indexes = [];
        List<string> texts = [];
        var embeddings = new Vector[snippetsTexts.Length];

        for (int i = 0; i < snippetsTexts.Length; i++)
        {
            var snippetText = snippetsTexts[i];
            var blockSize = blockSizesArray[i];

            if (excludeBlockSizes == null ||
                !excludeBlockSizes.Contains(blockSize))
            {
                indexes.Add(i);
                texts.Add(snippetText);
            }

        }

        if (texts.Count > 0)
        {
            var vectors = await EncodeBaseAsync(texts, cancellationToken);
            for (int i = 0; i < vectors.Length; i++)
                embeddings[indexes[i]] = vectors[i];
        }

        return embeddings;
    }

    private async Task<Vector[]> EncodeBaseAsync(IEnumerable<string> texts, CancellationToken cancellationToken = default)
    {
        Exception lastException = new Exception();
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
        
        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync("/v1/embeddings", new InfinityEmbeddingsArgs
                {
                    Model = ModelName,
                    Input = texts,
                }, linkedCts.Token);
                if (!response.IsSuccessStatusCode)
                    throw new Exception((await response.Content.ReadAsStringAsync(linkedCts.Token) ?? "").TruncateForLogging());

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadFromJsonAsync<InfinityEmbeddingsResult>(cancellationToken: linkedCts.Token);
                return content?.Data?.Select(t => t.Embedding).ToArray();
            }
            catch (Exception ex)
            {
                lastException = ex;
                if (attempt < 1) // Только для первой попытки
                {
                    try { await Task.Delay(1000, cancellationToken); }
                    catch (OperationCanceledException) { throw lastException; }
                }
            }
        }

        throw lastException;
    }

    private async Task<Vector> EncodeBaseAsync(string text, CancellationToken cancellationToken = default)
    {
        Exception lastException = new Exception();
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
        
        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync("/v1/embeddings", new InfinityEmbeddingsArgs
                {
                    Model = ModelName,
                    Input = [text],
                }, linkedCts.Token);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadFromJsonAsync<InfinityEmbeddingsResult>(cancellationToken: linkedCts.Token);
                return content?.Data?.First().Embedding;
            }
            catch (Exception ex)
            {
                lastException = ex;
                if (attempt < 1) // Только для первой попытки
                {
                    try { await Task.Delay(1000, cancellationToken); }
                    catch (OperationCanceledException) { throw lastException; }
                }
            }
        }

        throw lastException;
    }

    private async Task<Vector> EncodeQuestionBaseAsync(string query, CancellationToken cancellationToken = default)
    {
        Exception lastException = new Exception();
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
        
        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync("/v1/embeddings", new InfinityEmbeddingsArgs
                {
                    Model = ModelName,
                    Input = [GetDetailedInstruct(query)],
                }, linkedCts.Token);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadFromJsonAsync<InfinityEmbeddingsResult>(cancellationToken: linkedCts.Token);
                return content?.Data?.First().Embedding;
            }
            catch (Exception ex)
            {
                lastException = ex;
                if (attempt < 1) // Только для первой попытки
                {
                    try { await Task.Delay(1000, cancellationToken); }
                    catch (OperationCanceledException) { throw lastException; }
                }
            }
        }

        throw lastException;
    }
}
