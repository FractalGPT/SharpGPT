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
    //public static class DeepSeekV32
    //{
    //    /// <summary>
    //    /// 4,33tps throughput
    //    /// </summary>
    //    public const string Avian = "avian/fp8";

    //    /// <summary>
    //    /// 15,54tps throughput
    //    /// </summary>
    //    public const string Parasail = "parasail/fp8";

    //    /// <summary>
    //    /// 10,66tps throughput 
    //    /// </summary>
    //    public const string DeepInfra = "deepinfra/fp4";

    //    /// <summary>
    //    /// 33,23tps throughput
    //    /// </summary>
    //    public const string NovitaAI = "novita/fp8";


    //    /// <summary>
    //    /// Baseten (US, fp4) - Лучшая производительность: 0.34s latency, 210.2tps throughput, 97.9% uptime
    //    /// </summary>
    //    public const string Baseten = "baseten/fp4";

    //    /// <summary>
    //    /// GMICloud (US, fp8) - 1.27s latency, 24.31tps throughput, 94.9% uptime
    //    /// </summary>
    //    public const string GMICloud = "gmicloud/fp8";

    //    /// <summary>
    //    /// SiliconFlow (SG, fp8) - 7.73s latency, 64.83tps throughput, 92.2% uptime
    //    /// </summary>
    //    public const string SiliconFlow = "siliconflow/fp8";

    //    /// <summary>
    //    /// AtlasCloud (US, fp8) - 1.66s latency, 40.69tps throughput, 85.0% uptime
    //    /// </summary>
    //    public const string AtlasCloud = "atlas-cloud/fp8";

    //    /// <summary>
    //    /// Рекомендуемый список провайдеров (отсортировано по throughput - пропускной способности)
    //    /// Baseten указан дважды для 2 попыток перед fallback
    //    /// </summary>
    //    public static List<string> RecommendedOrder => new()
    //    {
    //        Baseten,        // 210.2 tps - ЛУЧШИЙ!
    //        SiliconFlow,    // 64.83 tps
    //        AtlasCloud,     // 40.69 tps
    //        GMICloud,       // 24.31 tps
    //        NovitaAI,       // 22.66 tps
    //        Avian,          // 10.32 tps
    //        DeepInfra       // throughput неизвестен, 76.7% uptime (худший)
    //    };
    //}

    /// <summary>
    /// Провайдеры для Claude Sonnet 4.5 (отсортировано по throughput - пропускной способности)
    /// 12.12.2025
    /// </summary>
    public static class ClaudeSonnet45
    {
        /// <summary>
        /// Throughput: 68,03tps
        /// </summary>
        public const string AmazonBedrock = "amazon-bedrock";

        /// <summary>
        /// Throughput: 60,96tps
        /// </summary>
        public const string Anthropic = "anthropic";

        /// <summary>
        /// Throughput: 55,68tps
        /// </summary>
        public const string GoogleVertexGlobal = "google-vertex/global";

        /// <summary>
        /// Throughput: 50,46tps
        /// </summary>
        public const string GoogleVertex = "google-vertex";

        /// <summary>
        /// Рекомендуемый список провайдеров (отсортировано по throughput - скорости генерации)
        /// </summary>
        public static List<string> RecommendedOrder =>
        [
            AmazonBedrock,
            Anthropic,
            GoogleVertexGlobal,
            GoogleVertex
        ];
    }

    /// <summary>
    /// Провайдеры для Claude Haiku 4.5 (отсортировано по throughput - пропускной способности)
    /// 12.12.2025
    /// </summary>
    public static class ClaudeHaiku45
    {
        /// <summary>
        /// Throughput: 130,2tps
        /// </summary>
        public const string GoogleVertex = "google-vertex";

        /// <summary>
        /// Throughput: 113,2tps
        /// </summary>
        public const string Anthropic = "anthropic";

        /// <summary>
        /// Throughput: 113,4tps
        /// </summary>
        public const string AmazonBedrock = "amazon-bedrock";

        /// <summary>
        /// Рекомендуемый список провайдеров (отсортировано по throughput - скорости генерации)
        /// </summary>
        public static List<string> RecommendedOrder =>
        [
            GoogleVertex,
            Anthropic,
            AmazonBedrock
        ];
    }



    /// <summary>
    /// Провайдеры для Gemini 3 Pro (отсортировано по throughput - пропускной способности)
    /// 12.12.2025
    /// </summary>
    public static class Gemini3Pro
    {
        /// <summary>
        /// Throughput: 71,49tps
        /// </summary>
        public const string GoogleAIStudio = "google-ai-studio";

        /// <summary>
        /// Throughput: 51,95tps
        /// </summary>
        public const string GoogleVertex = "google-vertex";

        /// <summary>
        /// Рекомендуемый список провайдеров (отсортировано по throughput - скорости генерации)
        /// </summary>
        public static List<string> RecommendedOrder =>
        [
            GoogleAIStudio,
            GoogleVertex
        ];
    }

    /// <summary>
    /// Провайдеры для Gemini 2.5 Pro (отсортировано по throughput - пропускной способности)
    /// 12.12.2025
    /// </summary>
    public static class Gemini25Pro
    {
        /// <summary>
        /// Throughput: 91,69tps
        /// </summary>
        public const string GoogleVertexGlobal = "google-vertex/global";

        /// <summary>
        /// Throughput: 73,04tps
        /// </summary>
        public const string GoogleAIStudio = "google-ai-studio";

        /// <summary>
        /// Throughput: 66,71tps
        /// </summary>
        public const string GoogleVertexUS = "google-vertex/us";

        /// <summary>
        /// Рекомендуемый список провайдеров (отсортировано по throughput - скорости генерации)
        /// </summary>
        public static List<string> RecommendedOrder =>
        [
            GoogleVertexGlobal,
            GoogleAIStudio,
            GoogleVertexUS
        ];
    }

    /// <summary>
    /// Провайдеры для Gemini 2.5 Flash (отсортировано по throughput - скорости генерации)
    /// 12.12.2025
    /// </summary>
    public static class Gemini25Flash
    {
        /// <summary>
        /// Throughput: 79,43tps
        /// </summary>
        public const string GoogleVertexGlobal = "google-vertex/global";

        /// <summary>
        /// Throughput: 78,75tps
        /// </summary>
        public const string GoogleVertex = "google-vertex";

        /// <summary>
        /// Throughput: 64,72tps
        /// </summary>
        public const string GoogleAIStudio = "google-ai-studio";

        /// <summary>
        /// Рекомендуемый список провайдеров (отсортировано по throughput - скорости генерации)
        /// </summary>
        public static List<string> RecommendedOrder =>
        [
            GoogleVertexGlobal,
            GoogleVertex,
            GoogleAIStudio
        ];
    }

    /// <summary>
    /// Провайдеры для Gemini 2.5 Flash Lite (отсортировано по throughput - скорости генерации)
    /// 12.12.2025
    /// </summary>
    public static class Gemini25FlashLite
    {
        /// <summary>
        /// Throughput: 67,31tps
        /// </summary>
        public const string GoogleVertex = "google-vertex";

        /// <summary>
        /// Throughput: 80,52tps
        /// </summary>
        public const string GoogleAIStudio = "google-ai-studio";

        /// <summary>
        /// Рекомендуемый список провайдеров (отсортировано по throughput - скорости генерации)
        /// </summary>
        public static List<string> RecommendedOrder =>
        [
            GoogleAIStudio,
            GoogleVertex,
        ];
    }



    /// <summary>
    /// Провайдеры для Gemini 2.0 Flash (отсортировано по throughput - скорости генерации)
    /// 12.12.2025
    /// </summary>
    public static class Gemini20Flash
    {
        /// <summary>
        /// Throughput: 124,0tps
        /// </summary>
        public const string GoogleVertex = "google-vertex";

        /// <summary>
        /// Throughput: 151,7tps
        /// </summary>
        public const string GoogleAIStudio = "google-ai-studio";

        /// <summary>
        /// Рекомендуемый список провайдеров (отсортировано по throughput - скорости генерации)
        /// </summary>
        public static List<string> RecommendedOrder =>
        [
            GoogleAIStudio,
            GoogleVertex
        ];
    }

    /// <summary>
    /// Провайдеры для Gemini 2.0 Flash Lite (отсортировано по throughput - скорости генерации)
    /// 12.12.2025
    /// </summary>
    public static class Gemini20FlashLite
    {
        /// <summary>
        /// Throughput: 177,9tps
        /// </summary>
        public const string GoogleVertex = "google-vertex";

        /// <summary>
        /// Throughput: 274,2tps
        /// </summary>
        public const string GoogleAIStudio = "google-ai-studio";

        /// <summary>
        /// Рекомендуемый список провайдеров (отсортировано по throughput - скорости генерации)
        /// </summary>
        public static List<string> RecommendedOrder =>
        [
            GoogleAIStudio,
            GoogleVertex
        ];
    }
}

