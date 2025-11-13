using FluentAssertions;
using FractalGPT.SharpGPTLib.Clients.OpenAI;
using Xunit;

namespace FractalGPT.SharpGPTLib.Tests.Clients;

public class ChatGptApiSimpleTests
{
    [Fact]
    public void Constructor_WithValidApiKey_ShouldCreateInstance()
    {
        // Arrange & Act
        var client = new ChatGptApi("test-api-key");

        // Assert
        client.Should().NotBeNull();
        client.ApiUrl.Should().Be("https://api.openai.com/v1/chat/completions");
    }

    [Fact]
    public void Constructor_WithCustomModel_ShouldSetModelName()
    {
        // Arrange & Act
        var client = new ChatGptApi("test-api-key", "gpt-4");

        // Assert
        client.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyApiKey_ShouldThrowArgumentNullException(string apiKey)
    {
        // Act
        var act = () => new ChatGptApi(apiKey);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}

