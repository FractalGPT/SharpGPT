using AI.DataStructs.Algebraic;

namespace FractalGPT.SharpGPTLib.Encoders.Reranker;

/// <summary>
/// Базовый (абстрактный) класс реранкера
/// </summary>
/// <typeparam name="QueryDataType">Тип данных запроса</typeparam>
/// <typeparam name="DocumentDataType">Тип данных документа</typeparam>
public abstract class RerankerBase<QueryDataType, DocumentDataType> : IRerankerService<QueryDataType, DocumentDataType>
{
    /// <summary>
    /// Мера схожести
    /// </summary>
    /// <param name="query"></param>
    /// <param name="outData"></param>
    /// <returns></returns>
    public abstract double Sim(QueryDataType query, DocumentDataType outData, string instruct = null);

    /// <summary>
    /// Мера схожести (Асинхронный метод)
    /// </summary>
    /// <param name="query"></param>
    /// <param name="outData"></param>
    /// <returns></returns>
    public abstract Task<double> SimAsync(QueryDataType query, DocumentDataType outData, string instruct = null);

    /// <summary>
    /// Выдает вектор близостей
    /// </summary>
    /// <param name="query"></param>
    /// <param name="documents"></param>
    /// <returns></returns>
    public abstract Task<Vector> SimsAsync(QueryDataType query, IEnumerable<DocumentDataType> documents, string instruct = null);

    /// <summary>
    /// Выдает вектор близостей
    /// </summary>
    /// <param name="query"></param>
    /// <param name="documents"></param>
    /// <returns></returns>
    public abstract Vector Sims(QueryDataType query, IEnumerable<DocumentDataType> documents, string instruct = null);

    /// <summary>
    /// Выдает топ-k вектор близостей с индексами
    /// </summary>
    /// <param name="query"></param>
    /// <param name="documents"></param>
    /// <param name="k">Количество наилучших результатов</param>
    /// <returns>Список кортежей (индекс, мера схожести)</returns>
    public virtual List<(int, double)> TopKSims(QueryDataType query, IEnumerable<DocumentDataType> documents, int k = 5, string instruct = null)
    {
        Vector sims = Sims(query, documents);
        DocumentDataType[] outDatasArray = documents.ToArray();

        List<(int, double)> sortedSims = outDatasArray
            .Select((data, index) => (index, sims[index]))
            .OrderByDescending(x => x.Item2)
            .Take(k)
            .ToList();

        return sortedSims;
    }

    /// <summary>
    /// Выдает топ-k вектор близостей с индексами (Асинхронный метод)
    /// </summary>
    /// <param name="query"></param>
    /// <param name="documents"></param>
    /// <param name="k">Количество наилучших результатов</param>
    /// <returns>Список кортежей (индекс, мера схожести)</returns>
    public virtual async Task<List<(int, double)>> TopKSimsAsync(QueryDataType query, IEnumerable<DocumentDataType> documents, int k = 5, string instruct = null)
    {
        Vector sims = await SimsAsync(query, documents);
        DocumentDataType[] outDatasArray = documents.ToArray();

        List<(int, double)> sortedSims = outDatasArray
            .Select((data, index) => (index, sims[index]))
            .OrderByDescending(x => x.Item2)
            .Take(k)
            .ToList();

        return sortedSims;
    }

}
