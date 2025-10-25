using AI.DataStructs.Algebraic;

namespace FractalGPT.SharpGPTLib.Core.Abstractions;

public interface IRerankerService<QueryDataType, DocumentDataType>
{
    /// <summary>
    /// Мера схожести
    /// </summary>
    /// <param name="query"></param>
    /// <param name="document"></param>
    /// <returns></returns>
    double Sim(QueryDataType query, DocumentDataType document, string instruct = null);

    /// <summary>
    /// Мера схожести (Асинхронный метод)
    /// </summary>
    /// <param name="query"></param>
    /// <param name="document"></param>
    /// <returns></returns>
    Task<double> SimAsync(QueryDataType query, DocumentDataType document, string instruct = null);

    /// <summary>
    /// Выдает вектор близостей
    /// </summary>
    /// <param name="query"></param>
    /// <param name="document"></param>
    /// <returns></returns>
    Task<Vector> SimsAsync(QueryDataType query, IEnumerable<DocumentDataType> document, string instruct = null);
}
