using FluentAssertions;
using FractalGPT.SharpGPTLib.Core.Models.Common.Requests;
using FractalGPT.SharpGPTLib.Core.Models.Common.Responses;
using FractalGPT.SharpGPTLib.Core.Models.Common.Messages;
using Xunit;

namespace FractalGPT.SharpGPTLib.Tests.Models;

public class SimpleModelsTests
{
    [Fact]
    public void Choice_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var choice = new Choice();

        // Assert
        choice.Should().NotBeNull();
    }

    [Fact]
    public void StreamOptions_ShouldSetIncludeUsage()
    {
        // Arrange & Act
        var options = new StreamOptions
        {
            IncludeUsage = true
        };

        // Assert
        options.IncludeUsage.Should().BeTrue();
    }

    [Fact]
    public void GenerateSettings_ShouldSetAllProperties()
    {
        // Arrange & Act
        var settings = new GenerateSettings
        {
            Temperature = 0.8,
            MaxTokens = 500,
            TopP = 0.95
        };

        // Assert
        settings.Temperature.Should().Be(0.8);
        settings.MaxTokens.Should().Be(500);
        settings.TopP.Should().Be(0.95);
    }

    [Fact]
    public void Usage_ShouldHaveTokenCounts()
    {
        // Arrange & Act
        var usage = new Usage
        {
            PromptTokens = 50,
            CompletionTokens = 100
        };

        // Assert
        usage.PromptTokens.Should().Be(50);
        usage.CompletionTokens.Should().Be(100);
        usage.TotalTokens.Should().Be(150);
    }

    [Fact]
    public void ChatCompletionsResponse_ShouldHaveId()
    {
        // Arrange & Act
        var response = new ChatCompletionsResponse
        {
            Id = "chatcmpl-123",
            Model = "gpt-4"
        };

        // Assert
        response.Id.Should().Be("chatcmpl-123");
        response.Model.Should().Be("gpt-4");
    }
}

