using FluentAssertions;
using FractalGPT.SharpGPTLib.Core.Models.Common.Generation;

namespace FractalGPT.SharpGPTLib.Tests.Models;

public class GenerationParametrsTests
{
    [Fact]
    public void GenerationParametrs_DefaultConstructor_ShouldSetDefaults()
    {
        // Act
        var params1 = new GenerationParametrs();

        // Assert
        params1.MaxLen.Should().Be(100);
        params1.Temperature.Should().BeApproximately(0.6, 0.01);
        params1.TopK.Should().Be(40);
        params1.TopP.Should().BeApproximately(0.9, 0.01);
        params1.NoRepeatNgramSize.Should().Be(3);
    }

    [Fact]
    public void GenerationParametrs_ParameterizedConstructor_ShouldSetCustomValues()
    {
        // Act
        var params1 = new GenerationParametrs(
            maxLen: 200,
            temperature: 0.8,
            topK: 50,
            topP: 0.95,
            noRepeatNgramSize: 4
        );

        // Assert
        params1.MaxLen.Should().Be(200);
        params1.Temperature.Should().BeApproximately(0.8, 0.01);
        params1.TopK.Should().Be(50);
        params1.TopP.Should().BeApproximately(0.95, 0.01);
        params1.NoRepeatNgramSize.Should().Be(4);
    }

    [Fact]
    public void GenerationParametrs_CanBeModifiedAfterCreation()
    {
        // Arrange
        var params1 = new GenerationParametrs();

        // Act
        params1.MaxLen = 500;
        params1.Temperature = 1.0;

        // Assert
        params1.MaxLen.Should().Be(500);
        params1.Temperature.Should().BeApproximately(1.0, 0.01);
    }
}

