using AI.DataStructs.Algebraic;

namespace FractalGPT.SharpGPTLib.Services.Embeddings.Base;

/// <summary>
/// Интерфейс для биэнкодера
/// </summary>
public interface IBiEncoder
{
    /// <summary>
    /// Преобразование текстов в векторы
    /// </summary>
    /// <param name="sentences">Тексты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns></returns>
    Task<Vector[]> EncodeAsync(IEnumerable<string> sentences, CancellationToken cancellationToken = default);
}
