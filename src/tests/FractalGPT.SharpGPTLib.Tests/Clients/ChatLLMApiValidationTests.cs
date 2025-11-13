using FluentAssertions;
using FractalGPT.SharpGPTLib.Clients.Base;

namespace FractalGPT.SharpGPTLib.Tests.Clients;

public class ChatLLMApiValidationTests
{
    [Theory]
    [InlineData(null, null)]
    [InlineData(0.5, 0.5)]
    [InlineData(1.5, 1.5)]
    [InlineData(2.0, 1.5)] // Should cap at 1.5
    [InlineData(-0.5, 0.0)] // Should floor at 0.0
    [InlineData(0.0, 0.0)]
    public void ValidateTemperature_ShouldClampValues(double? input, double? expected)
    {
        // Act
        var result = ChatLLMApi.ValidateTemperature(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(1, 1)]
    [InlineData(100, 100)]
    [InlineData(0, 1)] // Should be at least 1
    [InlineData(-10, 1)] // Should be at least 1
    public void ValidateMaxTokens_ShouldEnsureMinimumValue(int? input, int? expected)
    {
        // Act
        var result = ChatLLMApi.ValidateMaxTokens(input);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ValidateTemperature_WithExtremelyHighValue_ShouldCapAt1Point5()
    {
        // Arrange
        double? temperature = 100.0;

        // Act
        var result = ChatLLMApi.ValidateTemperature(temperature);

        // Assert
        result.Should().Be(1.5);
    }

    [Fact]
    public void ValidateTemperature_WithExtremelyLowValue_ShouldFloorAtZero()
    {
        // Arrange
        double? temperature = -100.0;

        // Act
        var result = ChatLLMApi.ValidateTemperature(temperature);

        // Assert
        result.Should().Be(0.0);
    }

    [Fact]
    public void ValidateMaxTokens_WithExtremelyLargeValue_ShouldKeepValue()
    {
        // Arrange
        int? maxTokens = 1000000;

        // Act
        var result = ChatLLMApi.ValidateMaxTokens(maxTokens);

        // Assert
        result.Should().Be(1000000);
    }
}

