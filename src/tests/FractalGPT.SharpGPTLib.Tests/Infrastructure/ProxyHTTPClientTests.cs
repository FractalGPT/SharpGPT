using FluentAssertions;
using FractalGPT.SharpGPTLib.Infrastructure.Http;
using System.Net;

namespace FractalGPT.SharpGPTLib.Tests.Infrastructure;

public class ProxyHTTPClientTests
{
    [Fact]
    public void Constructor_WithNullProxies_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new ProxyHTTPClient((IEnumerable<WebProxy>)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("proxies");
    }

    [Fact]
    public void Constructor_WithEmptyProxiesList_ShouldThrowArgumentException()
    {
        // Act
        var act = () => new ProxyHTTPClient(new List<WebProxy>());

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Список прокси не может быть пустым*")
            .WithParameterName("proxies");
    }

    [Fact]
    public void Constructor_WithNullProxyInList_ShouldThrowArgumentException()
    {
        // Arrange
        var proxies = new List<WebProxy> { null! };

        // Act
        var act = () => new ProxyHTTPClient(proxies);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*некорректный адрес*")
            .WithParameterName("proxies");
    }

    [Fact]
    public void Constructor_WithValidProxy_ShouldCreate()
    {
        // Arrange
        var proxy = new WebProxy("http://127.0.0.1:8080");
        var proxies = new List<WebProxy> { proxy };

        // Act
        var client = new ProxyHTTPClient(proxies);

        // Assert
        client.Should().NotBeNull();
        client.Authentication.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithApiKey_ShouldSetAuthentication()
    {
        // Arrange
        var proxy = new WebProxy("http://127.0.0.1:8080");
        var proxies = new List<WebProxy> { proxy };
        var apiKey = "test-api-key";

        // Act
        var client = new ProxyHTTPClient(proxies, apiKey);

        // Assert
        client.Should().NotBeNull();
        client.Authentication.Should().NotBeNull();
        client.Authentication!.Scheme.Should().Be("Bearer");
        client.Authentication.Parameter.Should().Be(apiKey);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidApiKey_ShouldThrowArgumentException(string invalidApiKey)
    {
        // Arrange
        var proxy = new WebProxy("http://127.0.0.1:8080");
        var proxies = new List<WebProxy> { proxy };

        // Act
        var act = () => new ProxyHTTPClient(proxies, invalidApiKey);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*API ключ не может быть пустым*")
            .WithParameterName("apiKey");
    }

    [Fact]
    public void GetWebProxy_WithValidProxyData_ShouldCreateWebProxy()
    {
        // Arrange
        var proxyData = new ProxyData
        {
            Address = "127.0.0.1",
            Port = 8080
        };

        // Act
        var proxy = ProxyHTTPClient.GetWebProxy(proxyData);

        // Assert
        proxy.Should().NotBeNull();
        proxy.Address.Should().NotBeNull();
        proxy.Address!.Host.Should().Be("127.0.0.1");
        proxy.Address.Port.Should().Be(8080);
    }

    [Fact]
    public void GetWebProxy_WithCredentials_ShouldSetCredentials()
    {
        // Arrange
        var proxyData = new ProxyData
        {
            Address = "127.0.0.1",
            Port = 8080,
            Login = "user",
            Password = "pass"
        };

        // Act
        var proxy = ProxyHTTPClient.GetWebProxy(proxyData);

        // Assert
        proxy.Should().NotBeNull();
        proxy.Credentials.Should().NotBeNull();
        proxy.UseDefaultCredentials.Should().BeFalse();
    }

    [Fact]
    public void GetWebProxy_WithNullProxyData_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => ProxyHTTPClient.GetWebProxy(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("proxyData");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetWebProxy_WithInvalidAddress_ShouldThrowArgumentException(string invalidAddress)
    {
        // Arrange
        var proxyData = new ProxyData
        {
            Address = invalidAddress,
            Port = 8080
        };

        // Act
        var act = () => ProxyHTTPClient.GetWebProxy(proxyData);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Адрес прокси не может быть пустым*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(65536)]
    [InlineData(100000)]
    public void GetWebProxy_WithInvalidPort_ShouldThrowArgumentException(int invalidPort)
    {
        // Arrange
        var proxyData = new ProxyData
        {
            Address = "127.0.0.1",
            Port = invalidPort
        };

        // Act
        var act = () => ProxyHTTPClient.GetWebProxy(proxyData);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"*Некорректный порт: {invalidPort}*");
    }

    [Fact]
    public void GetProxyStatistics_ShouldReturnStatistics()
    {
        // Arrange
        var proxy = new WebProxy("http://127.0.0.1:8080");
        var client = new ProxyHTTPClient(new List<WebProxy> { proxy });

        // Act
        var stats = client.GetProxyStatistics();

        // Assert
        stats.Should().NotBeNull();
        stats.Should().HaveCount(1);
        stats.First().ProxyAddress.Should().Contain("127.0.0.1");
    }

    [Fact]
    public void ResetProxyStatistics_ShouldClearAllStats()
    {
        // Arrange
        var proxy = new WebProxy("http://127.0.0.1:8080");
        var client = new ProxyHTTPClient(new List<WebProxy> { proxy });

        // Act
        client.ResetProxyStatistics();
        var stats = client.GetProxyStatistics();

        // Assert
        stats.First().FailureCount.Should().Be(0);
        stats.First().LastFailure.Should().BeNull();
        stats.First().LastSuccess.Should().BeNull();
    }

    [Fact]
    public void ValidateAllProxies_WithValidProxies_ShouldReturnEmptyErrors()
    {
        // Arrange
        var proxy = new WebProxy("http://127.0.0.1:8080");
        var client = new ProxyHTTPClient(new List<WebProxy> { proxy });

        // Act
        var errors = client.ValidateAllProxies();

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void Dispose_ShouldDisposeResources()
    {
        // Arrange
        var proxy = new WebProxy("http://127.0.0.1:8080");
        var client = new ProxyHTTPClient(new List<WebProxy> { proxy });

        // Act
        var act = () => client.Dispose();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ProxyHTTPClientOptions_DefaultValues_ShouldBeSet()
    {
        // Act
        var options = new ProxyHTTPClientOptions();

        // Assert
        options.AllowAutoRedirect.Should().BeTrue();
        options.UseCookies.Should().BeFalse();
        options.RequestTimeout.Should().Be(60);
        options.MaxConcurrentRequests.Should().Be(5);
        options.EnableDebugLogging.Should().BeFalse();
        options.DisableCertificateValidation.Should().BeFalse();
    }
}

