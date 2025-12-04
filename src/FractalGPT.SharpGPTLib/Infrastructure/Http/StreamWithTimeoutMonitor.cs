namespace FractalGPT.SharpGPTLib.Infrastructure.Http;

/// <summary>
/// Обертка над Stream, которая отслеживает время между получением данных
/// и выбрасывает TimeoutException если данные не поступают слишком долго.
/// 
/// ПОТОКОБЕЗОПАСНОСТЬ:
/// - Полностью потокобезопасен благодаря использованию Interlocked
/// - DateTime.UtcNow.Ticks хранится как long и обновляется атомарно
/// - Не требует lock для чтения/записи времени
/// </summary>
public class StreamWithTimeoutMonitor : Stream
{
    private readonly Stream _innerStream;
    private readonly TimeSpan _idleTimeout;
    private readonly CancellationToken _cancellationToken;
    
    /// <summary>
    /// Время последнего успешного чтения (в Ticks). 
    /// Используем long для атомарных операций через Interlocked.
    /// </summary>
    private long _lastReadTimeTicks;

    /// <summary>
    /// Создает новый экземпляр монитора потока с таймаутом простоя
    /// </summary>
    public StreamWithTimeoutMonitor(Stream innerStream, TimeSpan idleTimeout, CancellationToken cancellationToken)
    {
        _innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
        _idleTimeout = idleTimeout;
        _cancellationToken = cancellationToken;
        
        // Инициализируем время последнего чтения текущим моментом
        _lastReadTimeTicks = DateTime.UtcNow.Ticks;
    }

    public override bool CanRead => _innerStream.CanRead;
    public override bool CanSeek => _innerStream.CanSeek;
    public override bool CanWrite => _innerStream.CanWrite;
    public override long Length => _innerStream.Length;
    public override long Position
    {
        get => _innerStream.Position;
        set => _innerStream.Position = value;
    }

    public override void Flush() => _innerStream.Flush();

    public override int Read(byte[] buffer, int offset, int count)
    {
        CheckTimeout();
        var bytesRead = _innerStream.Read(buffer, offset, count);
        ResetTimer();
        return bytesRead;
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken, cancellationToken);
        
        // Проверяем таймаут перед началом чтения
        CheckTimeout();
        
        // Создаем задачу чтения
        var readTask = _innerStream.ReadAsync(buffer, offset, count, linkedCts.Token);
        
        // Вычисляем интервал проверки: минимум 100мс, максимум 1 секунда
        // Для коротких таймаутов проверяем чаще
        int checkIntervalMs = Math.Max(100, Math.Min(1000, (int)(_idleTimeout.TotalMilliseconds / 10)));
        
        // Ждем либо завершения чтения, либо таймаута
        while (!readTask.IsCompleted)
        {
            try
            {
                var delayTask = Task.Delay(checkIntervalMs, linkedCts.Token);
                var completedTask = await Task.WhenAny(readTask, delayTask);
                
                if (completedTask == readTask)
                    break;
                
                // Проверяем таймаут
                CheckTimeout();
            }
            catch (OperationCanceledException) when (linkedCts.IsCancellationRequested)
            {
                // Отмена - пробрасываем
                throw;
            }
        }
        
        // Получаем результат
        var bytesRead = await readTask;
        
        // Сбрасываем таймер
        ResetTimer();
        
        return bytesRead;
    }

    public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);
    public override void SetLength(long value) => _innerStream.SetLength(value);
    public override void Write(byte[] buffer, int offset, int count) => _innerStream.Write(buffer, offset, count);

    /// <summary>
    /// Атомарно сбрасывает таймер на текущее время
    /// </summary>
    private void ResetTimer()
    {
        Interlocked.Exchange(ref _lastReadTimeTicks, DateTime.UtcNow.Ticks);
    }

    /// <summary>
    /// Проверяет не превышен ли таймаут простоя
    /// </summary>
    private void CheckTimeout()
    {
        if (_cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException("Operation was cancelled", _cancellationToken);
        
        // Атомарно читаем время последнего чтения
        long lastTicks = Interlocked.Read(ref _lastReadTimeTicks);
        var lastReadTime = new DateTime(lastTicks, DateTimeKind.Utc);
        var elapsed = DateTime.UtcNow - lastReadTime;
        
        if (elapsed > _idleTimeout)
        {
            throw new TimeoutException(
                $"Stream idle timeout exceeded. No data received for {elapsed.TotalSeconds:F1} seconds " +
                $"(limit: {_idleTimeout.TotalSeconds} seconds). The request appears to be hung.");
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _innerStream?.Dispose();
        }
        base.Dispose(disposing);
    }
}
