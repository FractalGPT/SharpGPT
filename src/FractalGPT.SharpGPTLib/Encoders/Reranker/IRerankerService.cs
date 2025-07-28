using AI.DataStructs.Algebraic;

namespace FractalGPT.SharpGPTLib.Encoders.Reranker;

public interface IRerankerService<QueryDataType, DocumentDataType>
{
    /// <summary>
    /// Мера схожести
    /// </summary>
    /// <param name="query"></param>
    /// <param name="document"></param>
    /// <returns></returns>
    double Sim(QueryDataType query, DocumentDataType document);

    /// <summary>
    /// Мера схожести (Асинхронный метод)
    /// </summary>
    /// <param name="query"></param>
    /// <param name="document"></param>
    /// <returns></returns>
    Task<double> SimAsync(QueryDataType query, DocumentDataType document);

    /// <summary>
    /// Выдает вектор близостей
    /// </summary>
    /// <param name="query"></param>
    /// <param name="document"></param>
    /// <returns></returns>
    Task<Vector> SimsAsync(QueryDataType query, IEnumerable<DocumentDataType> document);
}
