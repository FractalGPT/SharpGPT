using AI.DataStructs.Algebraic;

namespace FractalGPT.SharpGPTLib.Encoders.Embedders;

/// <summary>
/// Интерфейс для биэнкодера
/// </summary>
public interface IBiEncoder
{
    /// <summary>
    /// Преобразование текстов в векторы
    /// </summary>
    /// <param name="sentences">Тексты</param>
    /// <returns></returns>
    Task<Vector[]> EncodeAsync(IEnumerable<string> sentences);
}
