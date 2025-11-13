using FluentAssertions;
using FractalGPT.SharpGPTLib.Core.Models.Common.Requests;

namespace FractalGPT.SharpGPTLib.Tests.Models;

public class GenerateSettingsTests
{
    [Fact]
    public void Constructor_WithDefaultValues_ShouldUseDefaults()
    {
        // Act
        var settings = new GenerateSettings();

        // Assert
        settings.Temperature.Should().Be(0.1);
        settings.RepetitionPenalty.Should().Be(1.05);
        settings.TopP.Should().Be(0.95);
        settings.TopK.Should().Be(20);
        settings.MinTokens.Should().Be(8);
        settings.MaxTokens.Should().Be(2500);
        settings.Stream.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithCustomValues_ShouldSetValues()
    {
        // Act
        var settings = new GenerateSettings(
            temperature: 0.8,
            repetitionPenalty: 1.1,
            topP: 0.9,
            topK: 40,
            minTokens: 10,
            maxTokens: 1000,
            streamId: "test-stream"
        );

        // Assert
        settings.Temperature.Should().Be(0.8);
        settings.RepetitionPenalty.Should().Be(1.1);
        settings.TopP.Should().Be(0.9);
        settings.TopK.Should().Be(40);
        settings.MinTokens.Should().Be(10);
        settings.MaxTokens.Should().Be(1000);
        settings.StreamId.Should().Be("test-stream");
        settings.Stream.Should().BeTrue(); // Stream should be true when StreamId is set
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("stream-id", true)]
    public void Stream_Property_ShouldBeBasedOnStreamId(string? streamId, bool expectedStream)
    {
        // Act
        var settings = new GenerateSettings(streamId: streamId);

        // Assert
        settings.Stream.Should().Be(expectedStream);
    }

    [Fact]
    public void ReasoningSettings_CanBeSet()
    {
        // Arrange
        var settings = new GenerateSettings();
        var reasoningSettings = new ReasoningSettings { MaxTokens = 5000 };

        // Act
        settings.ReasoningSettings = reasoningSettings;

        // Assert
        settings.ReasoningSettings.Should().NotBeNull();
        settings.ReasoningSettings.MaxTokens.Should().Be(5000);
    }

    [Theory]
    [InlineData("low")]
    [InlineData("medium")]
    [InlineData("high")]
    public void ReasoningEffort_CanBeSet(string effort)
    {
        // Arrange
        var settings = new GenerateSettings(reasoningEffort: effort);

        // Assert
        settings.ReasoningEffort.Should().Be(effort);
    }

    [Fact]
    public void StreamMethod_DefaultValue_ShouldBeStreamMessage()
    {
        // Act
        var settings = new GenerateSettings(streamId: "test");

        // Assert
        settings.StreamMethod.Should().Be("StreamMessage");
    }

    [Fact]
    public void LogProbs_CanBeEnabled()
    {
        // Arrange
        var settings = new GenerateSettings();

        // Act
        settings.LogProbs = true;
        settings.TopLogprobs = 5;

        // Assert
        settings.LogProbs.Should().BeTrue();
        settings.TopLogprobs.Should().Be(5);
    }
}

