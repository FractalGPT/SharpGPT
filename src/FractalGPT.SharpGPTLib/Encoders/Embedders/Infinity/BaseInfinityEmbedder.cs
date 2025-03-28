using AI.DataStructs.Algebraic;
using System.Net.Http.Json;

namespace FractalGPT.SharpGPTLib.Encoders.Embedders.Infinity;

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
        };
    }

    public Task<Vector[]> EncodeAsync(IEnumerable<string> texts) =>
        EncodeBaseAsync(texts);


    public Task<Vector> EncodeAsync(string text) =>
        EncodeBaseAsync(text);

    public Task<Vector> EncodeQuestionAsync(string text) =>
        EncodeQuestionBaseAsync(text);

    /// <summary>
    /// Нормализация косинуса через гиперболический тангенс
    /// </summary>
    public virtual double TanhCosineNormalize(double cosine) => 
        Math.Tanh(TanhNormParamK * (cosine - MeanCos) / StdCos + TanhNormParamB);

    public async Task<Vector[]> EncodeAsyncWithBlockSize(IEnumerable<string> processedTexts, IEnumerable<int> blockSizes, IEnumerable<int> excludeBlockSizes = null)
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
            var vectors = await EncodeBaseAsync(texts);
            for (int i = 0; i < vectors.Length; i++)
                embeddings[indexes[i]] = vectors[i];
        }

        return embeddings;
    }

    private async Task<Vector[]> EncodeBaseAsync(IEnumerable<string> texts)
    {
        Exception lastException = new Exception();
        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync("/v1/embeddings", new InfinityEmbeddingsArgs
                {
                    Model = ModelName,
                    Input = texts,
                });
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadFromJsonAsync<InfinityEmbeddingsResult>();
                return content?.Data?.Select(t => t.Embedding).ToArray();
            }
            catch (Exception ex)
            {
                lastException = ex;
                await Task.Delay(1000);
            }
        }

        throw lastException;
    }

    private async Task<Vector> EncodeBaseAsync(string text)
    {
        Exception lastException = new Exception();
        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync("/v1/embeddings", new InfinityEmbeddingsArgs
                {
                    Model = ModelName,
                    Input = [text],
                });
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadFromJsonAsync<InfinityEmbeddingsResult>();
                return content?.Data?.First().Embedding;
            }
            catch (Exception ex)
            {
                lastException = ex;
                await Task.Delay(1000);
            }
        }

        throw lastException;
    }

    private async Task<Vector> EncodeQuestionBaseAsync(string query)
    {
        Exception lastException = new Exception();
        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync("/v1/embeddings", new InfinityEmbeddingsArgs
                {
                    Model = ModelName,
                    Input = [GetDetailedInstruct(query)],
                });
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadFromJsonAsync<InfinityEmbeddingsResult>();
                return content?.Data?.First().Embedding;
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
