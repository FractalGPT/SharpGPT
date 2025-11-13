using FluentAssertions;
using FractalGPT.SharpGPTLib.Core.Models.Common.Messages;
using FractalGPT.SharpGPTLib.Core.Models.Common.Responses;

namespace FractalGPT.SharpGPTLib.Tests.Models;

public class ChatCompletionsResponseTests
{
    [Fact]
    public void DefaultConstructor_ShouldInitializeCollections()
    {
        // Act
        var response = new ChatCompletionsResponse();

        // Assert
        response.Choices.Should().NotBeNull().And.BeEmpty();
        response.Usage.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithContent_ShouldCreateResponse()
    {
        // Arrange
        var content = "Test response";

        // Act
        var response = new ChatCompletionsResponse(content);

        // Assert
        response.Choices.Should().HaveCount(1);
        response.Choices[0].Message.Should().NotBeNull();
        response.Choices[0].Message.Role.Should().Be("assistant");
        response.Choices[0].Message.Content.ToString().Should().Be(content);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        // Arrange
        var response = new ChatCompletionsResponse();

        // Act
        response.Id = "test-id";
        response.Provider = "openai";
        response.Model = "gpt-4";
        response.Object = "chat.completion";
        response.Created = 1234567890;

        // Assert
        response.Id.Should().Be("test-id");
        response.Provider.Should().Be("openai");
        response.Model.Should().Be("gpt-4");
        response.Object.Should().Be("chat.completion");
        response.Created.Should().Be(1234567890);
    }

    [Fact]
    public void Choices_CanBeAdded()
    {
        // Arrange
        var response = new ChatCompletionsResponse();
        var choice = new Choice
        {
            Message = LLMMessage.CreateMessage(Roles.Assistant, "Test"),
            Index = 0,
            FinishReason = "stop"
        };

        // Act
        response.Choices.Add(choice);

        // Assert
        response.Choices.Should().HaveCount(1);
        response.Choices[0].Should().Be(choice);
    }

    [Fact]
    public void Usage_CanBeSet()
    {
        // Arrange
        var response = new ChatCompletionsResponse();
        var usage = new Usage
        {
            PromptTokens = 10,
            CompletionTokens = 20,
            TotalTokens = 30
        };

        // Act
        response.Usage = usage;

        // Assert
        response.Usage.Should().Be(usage);
        response.Usage.PromptTokens.Should().Be(10);
        response.Usage.CompletionTokens.Should().Be(20);
        response.Usage.TotalTokens.Should().Be(30);
    }
}

