using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.Core.Models.Common.Requests;

/// <summary>
/// Настройки предпочтительного провайдера для OpenRouter
/// https://openrouter.ai/docs#provider-routing
/// </summary>
public class ProviderPreference
{
    /// <summary>
    /// Приоритетный список провайдеров (первый будет использован, если доступен)
    /// </summary>
    [JsonPropertyName("order")]
    public List<string> Order { get; set; }

    /// <summary>
    /// Разрешить fallback на другие провайдеры если указанный недоступен
    /// </summary>
    [JsonPropertyName("allow_fallbacks")]
    public bool? AllowFallbacks { get; set; }

    /// <summary>
    /// Фильтр по уровню квантования (например: "fp16", "fp8", "fp4")
    /// </summary>
    [JsonPropertyName("quantizations")]
    public List<string> Quantizations { get; set; }

    /// <summary>
    /// Список провайдеров для исключения
    /// </summary>
    [JsonPropertyName("ignore")]
    public List<string> Ignore { get; set; }

    /// <summary>
    /// Критерий сортировки провайдеров: "price" (цена), "latency" (задержка), "throughput" (пропускная способность)
    /// </summary>
    [JsonPropertyName("sort")]
    public string Sort { get; set; }

    /// <summary>
    /// Требовать, чтобы провайдер поддерживал все указанные параметры запроса
    /// </summary>
    [JsonPropertyName("require_parameters")]
    public bool? RequireParameters { get; set; }

    /// <summary>
    /// Создает настройки провайдера с одним предпочтительным провайдером
    /// </summary>
    public static ProviderPreference Create(string provider, bool allowFallbacks = true)
    {
        return new ProviderPreference
        {
            Order = new List<string> { provider },
            AllowFallbacks = allowFallbacks
        };
    }

    /// <summary>
    /// Создает настройки провайдера с несколькими провайдерами по приоритету
    /// </summary>
    public static ProviderPreference Create(List<string> providers, bool allowFallbacks = true)
    {
        return new ProviderPreference
        {
            Order = providers,
            AllowFallbacks = allowFallbacks
        };
    }
}

/// <summary>
/// Известные провайдеры для OpenRouter
/// </summary>
public static class OpenRouterProviders
{
    /// <summary>
    /// Провайдеры для DeepSeek V3.2 (отсортированы по производительности)
    /// </summary>
    public static class DeepSeekV32
    {
        /// <summary>
        /// Baseten (US, fp4) - Лучшая производительность: 0.34s latency, 210.2tps throughput, 97.9% uptime
        /// </summary>
        public const string Baseten = "Baseten";

        /// <summary>
        /// GMICloud (US, fp8) - 1.27s latency, 24.31tps throughput, 94.9% uptime
        /// </summary>
        public const string GMICloud = "GMICloud";

        /// <summary>
        /// SiliconFlow (SG, fp8) - 7.73s latency, 64.83tps throughput, 92.2% uptime
        /// </summary>
        public const string SiliconFlow = "SiliconFlow";

        /// <summary>
        /// Avian.io (US, fp8) - 1.83s latency, 10.32tps throughput, 98.3% uptime
        /// </summary>
        public const string Avian = "Avian.io";

        /// <summary>
        /// AtlasCloud (US, fp8) - 1.66s latency, 40.69tps throughput, 85.0% uptime
        /// </summary>
        public const string AtlasCloud = "AtlasCloud";

        /// <summary>
        /// NovitaAI (US, fp8) - 1.68s latency, 22.66tps throughput, 98.6% uptime
        /// </summary>
        public const string NovitaAI = "NovitaAI";

        /// <summary>
        /// DeepInfra (US, fp4) - latency неизвестна, throughput неизвестен, 76.7% uptime
        /// </summary>
        public const string DeepInfra = "DeepInfra";

        /// <summary>
        /// Рекомендуемый список провайдеров (отсортировано по throughput - пропускной способности)
        /// Baseten указан дважды для 2 попыток перед fallback
        /// </summary>
        public static List<string> RecommendedOrder => new()
        {
            Baseten,        // 210.2 tps - ЛУЧШИЙ!
            SiliconFlow,    // 64.83 tps
            AtlasCloud,     // 40.69 tps
            GMICloud,       // 24.31 tps
            NovitaAI,       // 22.66 tps
            Avian,          // 10.32 tps
            DeepInfra       // throughput неизвестен, 76.7% uptime (худший)
        };
    }

    /// <summary>
    /// Провайдеры для Claude Sonnet 4.5 (отсортировано по throughput - пропускной способности)
    /// </summary>
    public static class ClaudeSonnet45
    {
        /// <summary>
        /// Amazon Bedrock (US) - Максимальный throughput: 61.90 tps, 2.83s latency, 99.6% uptime
        /// </summary>
        public const string AmazonBedrock = "Amazon Bedrock";

        /// <summary>
        /// Anthropic (US) - Оригинальный провайдер: 60.77 tps, 3.17s latency, 100% uptime
        /// </summary>
        public const string Anthropic = "Anthropic";

        /// <summary>
        /// Google Vertex Global (US) - Хороший баланс: 53.95 tps, 1.46s latency, 100% uptime
        /// </summary>
        public const string GoogleVertexGlobal = "Google Vertex (Global)";

        /// <summary>
        /// Google Vertex (US) - Быстрый старт: 48.97 tps, 1.34s latency, 100% uptime
        /// </summary>
        public const string GoogleVertex = "Google Vertex";

        /// <summary>
        /// Рекомендуемый список провайдеров (отсортировано по throughput - скорости генерации)
        /// Amazon Bedrock указан дважды для 2 попыток перед fallback
        /// </summary>
        public static List<string> RecommendedOrder => new()
        {
            AmazonBedrock,          // 61.90 tps - МАКСИМАЛЬНЫЙ!
            Anthropic,              // 60.77 tps, оригинальный провайдер
            GoogleVertexGlobal,     // 53.95 tps
            GoogleVertex            // 48.97 tps, но быстрый старт
        };
    }

    /// <summary>
    /// Провайдеры для Claude Haiku 4.5 (отсортировано по throughput - пропускной способности)
    /// </summary>
    public static class ClaudeHaiku45
    {
        /// <summary>
        /// Google Vertex (US) - Максимальный throughput: 131.9 tps, 0.53s latency, 100% uptime
        /// </summary>
        public const string GoogleVertex = "Google Vertex";

        /// <summary>
        /// Anthropic (US) - Оригинальный провайдер: 110.3 tps, 1.49s latency, 98.8% uptime
        /// </summary>
        public const string Anthropic = "Anthropic";

        /// <summary>
        /// Amazon Bedrock (US) - Стабильный: 103.1 tps, 1.09s latency, 99.8% uptime
        /// </summary>
        public const string AmazonBedrock = "Amazon Bedrock";

        /// <summary>
        /// Рекомендуемый список провайдеров (отсортировано по throughput - скорости генерации)
        /// </summary>
        public static List<string> RecommendedOrder => new()
        {
            GoogleVertex,       // 131.9 tps - МАКСИМАЛЬНЫЙ!
            Anthropic,          // 110.3 tps, оригинальный провайдер
            AmazonBedrock       // 103.1 tps
        };
    }

    /// <summary>
    /// Провайдеры для Gemini 2.5 Pro (отсортировано по throughput - пропускной способности)
    /// </summary>
    public static class Gemini25Pro
    {
        /// <summary>
        /// Google Vertex (Global) (US) - Максимальный throughput: 100.6 tps, 2.24s latency, 98.8% uptime
        /// </summary>
        public const string GoogleVertexGlobal = "Google Vertex (Global)";

        /// <summary>
        /// Google Vertex (US) - Второй по throughput: 84.77 tps, 2.94s latency, 99.2% uptime
        /// </summary>
        public const string GoogleVertex = "Google Vertex (US)";

        /// <summary>
        /// Google AI Studio (US) - Третий по throughput: 81.45 tps, 2.95s latency, 96.6% uptime
        /// </summary>
        public const string GoogleAIStudio = "Google AI Studio";

        /// <summary>
        /// Рекомендуемый список провайдеров (отсортировано по throughput - скорости генерации)
        /// </summary>
        public static List<string> RecommendedOrder => new()
        {
            GoogleVertexGlobal,  // 100.6 tps - МАКСИМАЛЬНЫЙ!
            GoogleVertex,        // 84.77 tps
            GoogleAIStudio       // 81.45 tps
        };
    }
}

