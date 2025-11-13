using FluentAssertions;
using FractalGPT.SharpGPTLib.Services.LLM;
using FractalGPT.SharpGPTLib.Clients.Base;
using FractalGPT.SharpGPTLib.Core.Models.Common.Messages;

namespace FractalGPT.SharpGPTLib.Tests.Services;

public class LLMBaseTests
{
    [Fact]
    public void Constructor_WithValidChatLLMApi_ShouldCreate()
    {
        // Arrange
        var chatApi = new ChatLLMApi("test-key", "gpt-4", "prompt");

        // Act
        var llmBase = new LLMBase(chatApi);

        // Assert
        llmBase.Should().NotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SendToLLM_WithInvalidText_ShouldThrowArgumentException(string invalidText)
    {
        // Arrange
        var chatApi = new ChatLLMApi("test-key", "gpt-4", "prompt");
        var llmBase = new LLMBase(chatApi);

        // Act
        var act = async () => await llmBase.SendToLLM(invalidText);

        // Assert
        act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("text");
    }

    [Fact]
    public void SendToLLM_WithMessages_NullMessages_ShouldThrowArgumentNullException()
    {
        // Arrange
        var chatApi = new ChatLLMApi("test-key", "gpt-4", "prompt");
        var llmBase = new LLMBase(chatApi);

        // Act
        var act = async () => await llmBase.SendToLLM((IEnumerable<LLMMessage>)null!);

        // Assert
        act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("messages");
    }

    [Fact]
    public void TokenizeAsync_WithNullMessages_ShouldThrowArgumentNullException()
    {
        // Arrange
        var chatApi = new ChatLLMApi("test-key", "gpt-4", "prompt");
        var llmBase = new LLMBase(chatApi);

        // Act
        var act = async () => await llmBase.TokenizeAsync(null!);

        // Assert
        act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("messages");
    }
}

