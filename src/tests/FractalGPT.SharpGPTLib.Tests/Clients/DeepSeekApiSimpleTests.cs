using FluentAssertions;
using FractalGPT.SharpGPTLib.Clients.DeepSeek;
using Xunit;

namespace FractalGPT.SharpGPTLib.Tests.Clients;

public class DeepSeekApiSimpleTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var client = new DeepSeekApi("test-api-key", "deepseek-chat");

        // Assert
        client.Should().NotBeNull();
        client.ApiUrl.Should().Be("https://api.deepseek.com/v1/chat/completions");
    }

    [Fact]
    public void Constructor_WithCustomPrompt_ShouldCreateInstance()
    {
        // Arrange & Act
        var client = new DeepSeekApi("test-api-key", "deepseek-chat", prompt: "You are a helpful assistant");

        // Assert
        client.Should().NotBeNull();
    }
}

