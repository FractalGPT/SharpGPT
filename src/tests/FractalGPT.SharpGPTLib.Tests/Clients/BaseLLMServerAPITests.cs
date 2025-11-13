using FluentAssertions;
using FractalGPT.SharpGPTLib.API.LocalServer;
using FractalGPT.SharpGPTLib.Core.Models.Common.Generation;

namespace FractalGPT.SharpGPTLib.Tests.Clients;

public class BaseLLMServerAPITests
{
    [Fact]
    public void Constructor_WithDefaultHost_ShouldSetDefaultHost()
    {
        // Act
        var api = new BaseLLMServerAPI();

        // Assert
        api.Host.Should().Be("http://127.0.0.1:8080/");
    }

    [Fact]
    public void Constructor_WithCustomHost_ShouldSetCustomHost()
    {
        // Arrange
        var customHost = "http://localhost:8000/";

        // Act
        var api = new BaseLLMServerAPI(customHost);

        // Assert
        api.Host.Should().Be(customHost);
    }

    [Fact]
    public void Dispose_ShouldDisposeResources()
    {
        // Arrange
        var api = new BaseLLMServerAPI();

        // Act
        var act = () => api.Dispose();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var api = new BaseLLMServerAPI();

        // Act
        api.Dispose();
        var act = () => api.Dispose();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public async Task TextGeneration_WithGenerationParametrs_ShouldUseParameters()
    {
        // Arrange
        var api = new BaseLLMServerAPI("http://test-server/");
        var parameters = new GenerationParametrs
        {
            MaxLen = 100,
            Temperature = 0.8,
            TopK = 50,
            TopP = 0.9,
            NoRepeatNgramSize = 2
        };

        // Act & Assert
        // Этот тест проверяет только, что метод не выбрасывает исключение при валидных параметрах
        // Фактический HTTP-запрос будет неудачным, так как сервер не существует
        var act = async () => await api.TextGeneration("test prompt", parameters);
        
        // Мы ожидаем, что метод вернет null при ошибке сети
        var result = await act.Should().NotThrowAsync();
    }
}

