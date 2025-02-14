using AI.DataStructs.Algebraic;

namespace FractalGPT.SharpGPTLib.Encoders.Reranker;

/// <summary>
/// Базовый (абстрактный) класс реранкера
/// </summary>
/// <typeparam name="InpDataType">Тип данных запроса</typeparam>
/// <typeparam name="OutDataType">Тип данных ответа</typeparam>
public abstract class RerankerBase<InpDataType, OutDataType>
{
    /// <summary>
    /// Мера схожести
    /// </summary>
    /// <param name="query"></param>
    /// <param name="outData"></param>
    /// <returns></returns>
    public abstract double Sim(InpDataType query, OutDataType outData);

    /// <summary>
    /// Мера схожести (Асинхронный метод)
    /// </summary>
    /// <param name="query"></param>
    /// <param name="outData"></param>
    /// <returns></returns>
    public abstract Task<double> SimAsync(InpDataType query, OutDataType outData);

    /// <summary>
    /// Выдает вектор близостей
    /// </summary>
    /// <param name="query"></param>
    /// <param name="outData"></param>
    /// <returns></returns>
    public virtual async Task<Vector> SimsAsync(InpDataType query, IEnumerable<OutDataType> outData)
    {
        OutDataType[] outDatasArray = outData.ToArray();
        Vector sims = new Vector(outDatasArray.Length);

        for (int i = 0; i < outDatasArray.Length; i++)
            sims[i] = await SimAsync(query, outDatasArray[i]);

        return sims;
    }

    /// <summary>
    /// Выдает вектор близостей
    /// </summary>
    /// <param name="query"></param>
    /// <param name="outData"></param>
    /// <returns></returns>
    public virtual Vector Sims(InpDataType query, IEnumerable<OutDataType> outData)
    {
        OutDataType[] outDatasArray = outData.ToArray();
        Vector sims = new Vector(outDatasArray.Length);

        for (int i = 0; i < outDatasArray.Length; i++)
            sims[i] = Sim(query, outDatasArray[i]);

        return sims;
    }

    /// <summary>
    /// Выдает топ-k вектор близостей с индексами
    /// </summary>
    /// <param name="query"></param>
    /// <param name="outData"></param>
    /// <param name="k">Количество наилучших результатов</param>
    /// <returns>Список кортежей (индекс, мера схожести)</returns>
    public virtual List<(int, double)> TopKSims(InpDataType query, IEnumerable<OutDataType> outData, int k = 5)
    {
        Vector sims = Sims(query, outData);
        OutDataType[] outDatasArray = outData.ToArray();

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
    /// <param name="outData"></param>
    /// <param name="k">Количество наилучших результатов</param>
    /// <returns>Список кортежей (индекс, мера схожести)</returns>
    public virtual async Task<List<(int, double)>> TopKSimsAsync(InpDataType query, IEnumerable<OutDataType> outData, int k = 5)
    {
        Vector sims = await SimsAsync(query, outData);
        OutDataType[] outDatasArray = outData.ToArray();

        List<(int, double)> sortedSims = outDatasArray
            .Select((data, index) => (index, sims[index]))
            .OrderByDescending(x => x.Item2)
            .Take(k)
            .ToList();

        return sortedSims;
    }

}
