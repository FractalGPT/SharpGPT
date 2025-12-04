using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FractalGPT.SharpGPTLib.Clients.OpenRouter;
using FractalGPT.SharpGPTLib.Core.Models.Common.Messages;
using FractalGPT.SharpGPTLib.Core.Models.Common.Requests;
using FractalGPT.SharpGPTLib.Infrastructure.Http;
using Xunit;

namespace FractalGPT.SharpGPTLib.Tests.Infrastructure;

/// <summary>
/// Интеграционные тесты для проверки работы Idle Timeout с реальным API OpenRouter
/// ВНИМАНИЕ: Эти тесты требуют API ключ OpenRouter в переменной окружения OPENROUTER_API_KEY
/// Тесты пропускаются если ключ не установлен
/// </summary>
public class IdleTimeoutIntegrationTests
{
    private readonly string? _apiKey;
    private readonly bool _skipTests;

    public IdleTimeoutIntegrationTests()
    {
        _apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");
        _skipTests = string.IsNullOrEmpty(_apiKey);
    }

    /// <summary>
    /// Тест: Проверка работы idle timeout с реальным API запросом к OpenRouter с моделью Flash 1.5
    /// Обычный запрос (без стриминга) должен работать без ошибок
    /// </summary>
    [Fact]
    public async Task OpenRouter_Flash15_NonStreaming_WithIdleTimeout_ShouldWork()
    {
        // Skip if API key is not available
        if (_skipTests)
        {
            return; // Xunit пропустит этот тест
        }

        // Arrange
        var api = new OpenRouterModelApi(_apiKey!, "google/gemini-flash-1.5");
        
        // Устанавливаем idle timeout в 30 секунд
        api.IdleTimeoutSettings = IdleTimeoutSettings.FromSeconds(30);
        
        var messages = new[]
        {
            new LLMMessage("user", "Привет! Напиши короткий ответ в одно предложение.")
        };

        var settings = new GenerateSettings(
            temperature: 0.7,
            maxTokens: 50,
            streamId: null // Без стриминга
        );

        // Act
        var response = await api.SendWithContextAsync(messages, settings, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Choices);
        Assert.NotEmpty(response.Choices);
        Assert.NotNull(response.Choices.First().Message);
        
        // Проверяем что ответ содержит текст
        var content = response.Choices.First().Message.Content;
        Assert.NotNull(content);
        Assert.True(content.ToString().Length > 0, "Response content should not be empty");
    }

    /// <summary>
    /// Тест: Проверка что idle timeout срабатывает при отключении
    /// </summary>
    [Fact]
    public async Task OpenRouter_Flash15_WithDisabledIdleTimeout_ShouldWork()
    {
        // Skip if API key is not available
        if (_skipTests)
        {
            return;
        }

        // Arrange
        var api = new OpenRouterModelApi(_apiKey!, "google/gemini-flash-1.5");
        
        // Отключаем idle timeout
        api.IdleTimeoutSettings = IdleTimeoutSettings.Disabled;
        
        var messages = new[]
        {
            new LLMMessage("user", "Привет!")
        };

        var settings = new GenerateSettings(
            temperature: 0.7,
            maxTokens: 20,
            streamId: null
        );

        // Act
        var response = await api.SendWithContextAsync(messages, settings, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Choices);
        Assert.NotEmpty(response.Choices);
    }

    /// <summary>
    /// Тест: Проверка работы с более длинным ответом
    /// </summary>
    [Fact]
    public async Task OpenRouter_Flash15_LongResponse_WithIdleTimeout_ShouldWork()
    {
        // Skip if API key is not available
        if (_skipTests)
        {
            return;
        }

        // Arrange
        var api = new OpenRouterModelApi(_apiKey!, "google/gemini-flash-1.5");
        
        // Устанавливаем idle timeout в 45 секунд для более длинного ответа
        api.IdleTimeoutSettings = IdleTimeoutSettings.FromSeconds(45);
        
        var messages = new[]
        {
            new LLMMessage("user", "Расскажи про погоду в 2-3 предложениях.")
        };

        var settings = new GenerateSettings(
            temperature: 0.7,
            maxTokens: 200,
            streamId: null
        );

        // Act
        var response = await api.SendWithContextAsync(messages, settings, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Choices);
        Assert.NotEmpty(response.Choices);
        
        var content = response.Choices.First().Message.Content;
        Assert.NotNull(content);
        Assert.True(content.ToString().Length > 10, "Response should contain meaningful content");
    }

    /// <summary>
    /// Тест: Проверка что короткий idle timeout НЕ вызывает ложных срабатываний для быстрых ответов
    /// </summary>
    [Fact]
    public async Task OpenRouter_Flash15_ShortIdleTimeout_FastResponse_ShouldWork()
    {
        // Skip if API key is not available
        if (_skipTests)
        {
            return;
        }

        // Arrange
        var api = new OpenRouterModelApi(_apiKey!, "google/gemini-flash-1.5");
        
        // Устанавливаем короткий idle timeout в 10 секунд
        // Для быстрого ответа этого должно быть достаточно
        api.IdleTimeoutSettings = IdleTimeoutSettings.FromSeconds(10);
        
        var messages = new[]
        {
            new LLMMessage("user", "Ответь одним словом: да")
        };

        var settings = new GenerateSettings(
            temperature: 0.0, // Низкая температура для детерминированности
            maxTokens: 5,
            streamId: null
        );

        // Act
        var response = await api.SendWithContextAsync(messages, settings, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Choices);
        Assert.NotEmpty(response.Choices);
    }
}
