using FluentAssertions;
using FractalGPT.SharpGPTLib.Infrastructure.Http;

namespace FractalGPT.SharpGPTLib.Tests.Infrastructure;

public class WithoutProxyClientTests
{
    [Fact]
    public void Constructor_WithApiKey_ShouldSetApiKey()
    {
        // Arrange
        var apiKey = "test-api-key";

        // Act
        var client = new WithoutProxyClient(apiKey);

        // Assert
        client.ApiKey.Should().Be(apiKey);
        client.Authentication.Should().NotBeNull();
        client.Authentication!.Scheme.Should().Be("Bearer");
        client.Authentication.Parameter.Should().Be(apiKey);
    }

    [Fact]
    public void Constructor_WithNullApiKey_ShouldNotSetAuthentication()
    {
        // Act
        var client = new WithoutProxyClient(null!);

        // Assert
        client.ApiKey.Should().BeNull();
        client.Authentication.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithEmptyApiKey_ShouldNotSetAuthentication()
    {
        // Act
        var client = new WithoutProxyClient(string.Empty);

        // Assert
        client.ApiKey.Should().Be(string.Empty);
        client.Authentication.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithoutParameters_ShouldCreateClient()
    {
        // Act
        var client = new WithoutProxyClient();

        // Assert
        client.Should().NotBeNull();
        client.ApiKey.Should().BeNull();
        client.Authentication.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnNewInstance()
    {
        // Arrange
        var apiKey = "test-api-key";

        // Act
        var client = await WithoutProxyClient.CreateAsync(apiKey);

        // Assert
        client.Should().NotBeNull();
        client.ApiKey.Should().Be(apiKey);
    }

    [Fact]
    public void Dispose_ShouldDisposeHttpClient()
    {
        // Arrange
        var client = new WithoutProxyClient("test-key");

        // Act
        var act = () => client.Dispose();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var client = new WithoutProxyClient("test-key");

        // Act
        client.Dispose();
        var act = () => client.Dispose();

        // Assert
        act.Should().NotThrow();
    }
}

