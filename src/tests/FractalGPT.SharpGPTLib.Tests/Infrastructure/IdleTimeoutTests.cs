using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FractalGPT.SharpGPTLib.Infrastructure.Http;
using Xunit;

namespace FractalGPT.SharpGPTLib.Tests.Infrastructure;

/// <summary>
/// Тесты для проверки работы Idle Timeout механизма
/// </summary>
public class IdleTimeoutTests
{
    /// <summary>
    /// Тест: StreamWithTimeoutMonitor выбрасывает TimeoutException если данные не поступают
    /// </summary>
    [Fact]
    public async Task StreamWithTimeoutMonitor_ThrowsTimeoutException_WhenNoDataReceived()
    {
        // Arrange: Создаем поток, который ничего не отправляет
        var slowStream = new SlowStream(delayMilliseconds: 5000); // 5 секунд задержка
        var idleTimeout = TimeSpan.FromSeconds(2); // Таймаут 2 секунды
        var cts = new CancellationTokenSource();
        
        var monitoredStream = new StreamWithTimeoutMonitor(slowStream, idleTimeout, cts.Token);

        // Act & Assert: Ожидаем TimeoutException при попытке чтения
        var buffer = new byte[1024];
        await Assert.ThrowsAsync<TimeoutException>(async () =>
        {
            await monitoredStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
        });
    }

    /// <summary>
    /// Тест: StreamWithTimeoutMonitor НЕ выбрасывает исключение если данные поступают вовремя
    /// </summary>
    [Fact]
    public async Task StreamWithTimeoutMonitor_DoesNotThrow_WhenDataReceivedOnTime()
    {
        // Arrange: Создаем поток с быстрым откликом
        var fastStream = new SlowStream(delayMilliseconds: 100); // 100мс задержка
        var idleTimeout = TimeSpan.FromSeconds(5); // Таймаут 5 секунд
        var cts = new CancellationTokenSource();
        
        var monitoredStream = new StreamWithTimeoutMonitor(fastStream, idleTimeout, cts.Token);

        // Act: Читаем данные
        var buffer = new byte[1024];
        var bytesRead = await monitoredStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);

        // Assert: Данные получены без ошибок
        Assert.True(bytesRead > 0);
    }

    /// <summary>
    /// Тест: StreamWithTimeoutMonitor сбрасывает таймер при получении данных
    /// </summary>
    [Fact]
    public async Task StreamWithTimeoutMonitor_ResetsTimer_AfterReceivingData()
    {
        // Arrange: Поток с периодической отправкой данных
        var periodicStream = new PeriodicStream(intervalMilliseconds: 500, totalChunks: 3); // 3 чанка по 0.5сек
        var idleTimeout = TimeSpan.FromSeconds(3); // Таймаут 3 секунды (больше чем интервал)
        var cts = new CancellationTokenSource();
        
        var monitoredStream = new StreamWithTimeoutMonitor(periodicStream, idleTimeout, cts.Token);

        // Act: Читаем все данные
        var buffer = new byte[1024];
        int totalBytesRead = 0;
        
        // Первое чтение
        var bytesRead1 = await monitoredStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
        totalBytesRead += bytesRead1;
        
        // Второе чтение (таймер должен был сброситься)
        var bytesRead2 = await monitoredStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
        totalBytesRead += bytesRead2;
        
        // Третье чтение
        var bytesRead3 = await monitoredStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
        totalBytesRead += bytesRead3;

        // Assert: Все данные получены без TimeoutException
        Assert.True(totalBytesRead > 0);
    }

    /// <summary>
    /// Тест: IdleTimeoutSettings.FromSeconds создает корректные настройки
    /// </summary>
    [Fact]
    public void IdleTimeoutSettings_FromSeconds_CreatesCorrectSettings()
    {
        // Act
        var settings = IdleTimeoutSettings.FromSeconds(45);

        // Assert
        Assert.True(settings.Enabled);
        Assert.Equal(TimeSpan.FromSeconds(45), settings.IdleTimeout);
    }

    /// <summary>
    /// Тест: IdleTimeoutSettings.Disabled отключает мониторинг
    /// </summary>
    [Fact]
    public void IdleTimeoutSettings_Disabled_IsCorrect()
    {
        // Act
        var settings = IdleTimeoutSettings.Disabled;

        // Assert
        Assert.False(settings.Enabled);
    }

    /// <summary>
    /// Тест: IdleTimeoutSettings.Default имеет корректные значения по умолчанию
    /// </summary>
    [Fact]
    public void IdleTimeoutSettings_Default_HasCorrectValues()
    {
        // Act
        var settings = IdleTimeoutSettings.Default;

        // Assert
        Assert.True(settings.Enabled);
        Assert.Equal(TimeSpan.FromSeconds(30), settings.IdleTimeout);
    }

    /// <summary>
    /// Тест: StreamWithTimeoutMonitor потокобезопасен при конкурентных вызовах ReadAsync
    /// </summary>
    [Fact]
    public async Task StreamWithTimeoutMonitor_IsThreadSafe_WithConcurrentReads()
    {
        // Arrange: Создаем поток с быстрым откликом
        var fastStream = new ConcurrentReadStream();
        var idleTimeout = TimeSpan.FromSeconds(5);
        var cts = new CancellationTokenSource();
        
        var monitoredStream = new StreamWithTimeoutMonitor(fastStream, idleTimeout, cts.Token);

        // Act: Запускаем 10 параллельных чтений
        var tasks = new List<Task<int>>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                var buffer = new byte[1024];
                return await monitoredStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
            }));
        }

        // Assert: Все чтения должны завершиться без исключений
        var results = await Task.WhenAll(tasks);
        Assert.All(results, bytesRead => Assert.True(bytesRead > 0));
    }
}

/// <summary>
/// Вспомогательный класс: поток с задержкой перед отправкой данных
/// </summary>
internal class SlowStream : Stream
{
    private readonly int _delayMilliseconds;
    private bool _dataRead = false;

    public SlowStream(int delayMilliseconds)
    {
        _delayMilliseconds = delayMilliseconds;
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (_dataRead)
            return 0; // EOF

        await Task.Delay(_delayMilliseconds, cancellationToken);

        _dataRead = true;
        var data = Encoding.UTF8.GetBytes("test data");
        Array.Copy(data, 0, buffer, offset, Math.Min(data.Length, count));
        return data.Length;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return ReadAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
    }

    public override void Flush() { }
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}

/// <summary>
/// Вспомогательный класс: поток с периодической отправкой чанков данных
/// </summary>
internal class PeriodicStream : Stream
{
    private readonly int _intervalMilliseconds;
    private readonly int _totalChunks;
    private int _chunksRead = 0;

    public PeriodicStream(int intervalMilliseconds, int totalChunks)
    {
        _intervalMilliseconds = intervalMilliseconds;
        _totalChunks = totalChunks;
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (_chunksRead >= _totalChunks)
            return 0; // EOF

        await Task.Delay(_intervalMilliseconds, cancellationToken);

        _chunksRead++;
        var data = Encoding.UTF8.GetBytes($"chunk{_chunksRead}");
        Array.Copy(data, 0, buffer, offset, Math.Min(data.Length, count));
        return data.Length;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return ReadAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
    }

    public override void Flush() { }
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}

/// <summary>
/// Вспомогательный класс: поток для тестирования конкурентного доступа
/// </summary>
internal class ConcurrentReadStream : Stream
{
    private int _readCount = 0;
    private readonly object _lock = new object();

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        // Небольшая задержка для имитации реальной работы
        await Task.Delay(10, cancellationToken);

        lock (_lock)
        {
            _readCount++;
            if (_readCount > 20)
                return 0; // EOF после 20 чтений

            var data = Encoding.UTF8.GetBytes($"data{_readCount}");
            Array.Copy(data, 0, buffer, offset, Math.Min(data.Length, count));
            return data.Length;
        }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return ReadAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
    }

    public override void Flush() { }
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}

