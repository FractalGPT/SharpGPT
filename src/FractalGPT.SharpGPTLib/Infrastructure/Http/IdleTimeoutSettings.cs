namespace FractalGPT.SharpGPTLib.Infrastructure.Http;

/// <summary>
/// Настройки для мониторинга таймаута простоя (idle timeout) при чтении потока данных
/// 
/// ПОТОКОБЕЗОПАСНОСТЬ:
/// - Класс НЕ является полностью потокобезопасным для записи (сеттеры)
/// - TimeSpan и bool читаются атомарно (value types)
/// - Рекомендуется устанавливать настройки перед началом запроса и не изменять во время выполнения
/// - При конкурентном доступе используйте snapshot: var settings = chatApi.IdleTimeoutSettings;
/// </summary>
public class IdleTimeoutSettings
{
    /// <summary>
    /// Максимальное время простоя между получением данных (по умолчанию 40 секунд)
    /// </summary>
    public TimeSpan IdleTimeout { get; set; } = TimeSpan.FromSeconds(40);

    /// <summary>
    /// Включен ли мониторинг таймаута простоя (по умолчанию true)
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Настройки по умолчанию (30 секунд)
    /// </summary>
    public static IdleTimeoutSettings Default => new IdleTimeoutSettings();

    /// <summary>
    /// Создает настройки с отключенным мониторингом
    /// </summary>
    public static IdleTimeoutSettings Disabled => new IdleTimeoutSettings { Enabled = false };

    /// <summary>
    /// Создает настройки с указанным временем простоя
    /// </summary>
    public static IdleTimeoutSettings FromSeconds(int seconds) => new IdleTimeoutSettings
    {
        IdleTimeout = TimeSpan.FromSeconds(seconds),
        Enabled = true
    };

    public static IdleTimeoutSettings FromTimeSpan(TimeSpan timeSpan) => new IdleTimeoutSettings
    {
        IdleTimeout = timeSpan,
        Enabled = true,
    };
}

