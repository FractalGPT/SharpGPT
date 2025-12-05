using AI.DataStructs.Algebraic;
using FractalGPT.SharpGPTLib.Services.Embeddings.Base;

namespace FractalGPT.SharpGPTLib.Core.Abstractions;

/// <summary>
/// Определяет сервис для генерации векторных эмбеддингов на основе текстовых данных.
/// </summary>
public interface IEmbedderService : IBiEncoder
{
    /// <summary>
    /// Асинхронно генерирует векторное представление (эмбеддинг) для заданного вопроса.
    /// </summary>
    /// <param name="question">Вопрос в виде строки, для которого необходимо получить эмбеддинг.</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Задача, представляющая асинхронную операцию, результатом которой является вектор эмбеддинга вопроса.</returns>
    Task<Vector> EncodeQuestionAsync(string question, CancellationToken cancellationToken = default);

    /// <summary>
    /// Асинхронно генерирует векторное представление для переданного текста.
    /// </summary>
    /// <param name="text">Текст, для которого требуется сгенерировать эмбеддинг.</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Задача, представляющая асинхронную операцию, результатом которой является вектор эмбеддинга текста.</returns>
    Task<Vector> EncodeAsync(string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Асинхронно генерирует векторные представления для коллекции текстов.
    /// </summary>
    /// <param name="texts">Коллекция строк, для которых необходимо получить эмбеддинги.</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Задача, представляющая асинхронную операцию, результатом которой является массив векторов эмбеддингов для каждого текста.</returns>
    Task<Vector[]> EncodeAsync(IEnumerable<string> texts, CancellationToken cancellationToken = default);

    /// <summary>
    /// Асинхронно генерирует векторные представления для коллекции обработанных текстов с учётом заданных размеров блоков.
    /// </summary>
    /// <param name="processedTexts">Коллекция обработанных текстов, для которых генерируются эмбеддинги.</param>
    /// <param name="blockSizes">Коллекция размеров блоков, соответствующая каждому тексту.</param>
    /// <param name="excludeBlockSizes">
    /// Необязательная коллекция размеров блоков, которые следует исключить из обработки.
    /// Если значение <c>null</c>, исключения не применяются.
    /// </param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Задача, представляющая асинхронную операцию, результатом которой является массив векторов эмбеддингов с учетом заданных блоков.</returns>
    Task<Vector[]> EncodeAsyncWithBlockSize(
        IEnumerable<string> processedTexts,
        IEnumerable<int> blockSizes,
        IEnumerable<int> excludeBlockSizes = null,
        CancellationToken cancellationToken = default);

    double TanhCosineNormalize(double cosine);
}
