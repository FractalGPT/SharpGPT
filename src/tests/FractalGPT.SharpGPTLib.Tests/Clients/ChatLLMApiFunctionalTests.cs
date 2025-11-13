using FluentAssertions;
using FractalGPT.SharpGPTLib.Clients.OpenAI;
using FractalGPT.SharpGPTLib.Core.Models.Common.Requests;
using FractalGPT.SharpGPTLib.Core.Models.Common.Messages;
using Xunit;

namespace FractalGPT.SharpGPTLib.Tests.Clients;

public class ChatLLMApiFunctionalTests
{
    [Fact]
    public void GetSendDataAsync_WithTextAndSettings_ShouldCreateSendData()
    {
        // Arrange
        var client = new ChatGptApi("test-api-key");
        var settings = new GenerateSettings
        {
            Temperature = 0.7,
            MaxTokens = 100
        };

        // Act
        var sendData = client.GetSendDataAsync("Hello world", settings);

        // Assert
        sendData.Should().NotBeNull();
        sendData.Messages.Should().HaveCount(1);
    }

    [Fact]
    public void GetSendDataAsync_WithOnlyText_ShouldUseDefaultSettings()
    {
        // Arrange
        var client = new ChatGptApi("test-api-key");

        // Act
        var sendData = client.GetSendDataAsync("Test message");

        // Assert
        sendData.Should().NotBeNull();
        sendData.Messages.Should().HaveCount(1);
        sendData.Messages[0].Content.ToString().Should().Be("Test message");
    }

    [Fact]
    public void GetSendDataAsync_WithMultipleMessages_ShouldIncludeAllMessages()
    {
        // Arrange
        var client = new ChatGptApi("test-api-key", prompt: "You are helpful");

        // Act
        var sendData = client.GetSendDataAsync("User message");

        // Assert
        sendData.Should().NotBeNull();
        sendData.Messages.Should().HaveCountGreaterOrEqualTo(1);
    }
}

