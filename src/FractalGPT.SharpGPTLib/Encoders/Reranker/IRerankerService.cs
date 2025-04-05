using AI.DataStructs.Algebraic;

namespace FractalGPT.SharpGPTLib.Encoders.Reranker;

public interface IRerankerService<InpDataType, OutDataType>
{
    /// <summary>
    /// Мера схожести
    /// </summary>
    /// <param name="query"></param>
    /// <param name="outData"></param>
    /// <returns></returns>
    double Sim(InpDataType query, OutDataType outData);

    /// <summary>
    /// Мера схожести (Асинхронный метод)
    /// </summary>
    /// <param name="query"></param>
    /// <param name="outData"></param>
    /// <returns></returns>
    Task<double> SimAsync(InpDataType query, OutDataType outData);

    /// <summary>
    /// Выдает вектор близостей
    /// </summary>
    /// <param name="query"></param>
    /// <param name="outData"></param>
    /// <returns></returns>
    Task<Vector> SimsAsync(InpDataType query, IEnumerable<OutDataType> outData);
}
