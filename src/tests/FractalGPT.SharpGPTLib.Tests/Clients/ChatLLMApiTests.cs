using FluentAssertions;
using FractalGPT.SharpGPTLib.API.LLMAPI;
using FractalGPT.SharpGPTLib.Clients.Base;
using FractalGPT.SharpGPTLib.Core.Abstractions;
using FractalGPT.SharpGPTLib.Core.Models.Common.Messages;
using FractalGPT.SharpGPTLib.Core.Models.Common.Requests;
using FractalGPT.SharpGPTLib.Infrastructure.Http;
using NSubstitute;
using System.Net;
using System.Reflection;

namespace FractalGPT.SharpGPTLib.Tests.Clients;

public class ChatLLMApiTests
{
    [Fact]
    public void Constructor_WithNullModelName_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new ChatLLMApi(apiKey: "test", modelName: null!, prompt: "test");

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("modelName");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyModelName_ShouldThrowArgumentNullException(string emptyModelName)
    {
        // Act
        var act = () => new ChatLLMApi(apiKey: "test", modelName: emptyModelName, prompt: "test");

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("modelName");
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldSetModelName()
    {
        // Arrange
        var modelName = "gpt-4";
        var prompt = "You are a helpful assistant";
        var apiKey = "test-key";

        // Act
        var api = new ChatLLMApi(apiKey, modelName, prompt);

        // Assert
        api.ModelName.Should().Be(modelName);
    }

    [Fact]
    public void Validate_WithNullSettings_ShouldReturnNewSettings()
    {
        // Arrange
        var api = new ChatLLMApi("test", "gpt-4", "prompt");

        // Act
        var settings = api.Validate(null);

        // Assert
        settings.Should().NotBeNull();
    }

    [Fact]
    public void Validate_ShouldValidateTemperature()
    {
        // Arrange
        var api = new ChatLLMApi("test", "gpt-4", "prompt");
        var settings = new GenerateSettings(temperature: 2.0); // Over limit

        // Act
        var validated = api.Validate(settings);

        // Assert
        validated.Temperature.Should().Be(1.5); // Should be capped
    }

    [Fact]
    public void Validate_ShouldValidateMaxTokens()
    {
        // Arrange
        var api = new ChatLLMApi("test", "gpt-4", "prompt");
        var settings = new GenerateSettings(maxTokens: -10); // Invalid

        // Act
        var validated = api.Validate(settings);

        // Assert
        validated.MaxTokens.Should().Be(1); // Should be at least 1
    }

    [Fact]
    public void Validate_WithReasoningSettings_ShouldValidateReasoningMaxTokens()
    {
        // Arrange
        var api = new ChatLLMApi("test", "gpt-4", "prompt");
        var settings = new GenerateSettings();
        settings.ReasoningSettings = new ReasoningSettings { MaxTokens = -10 };

        // Act
        var validated = api.Validate(settings);

        // Assert
        validated.ReasoningSettings.MaxTokens.Should().Be(1);
    }

    [Fact]
    public void GetSendDataAsync_ShouldCreateSendData()
    {
        // Arrange
        var api = new ChatLLMApi("test", "gpt-4", "You are helpful");
        var text = "Hello";

        // Act
        var sendData = api.GetSendDataAsync(text);

        // Assert
        sendData.Should().NotBeNull();
        sendData.ModelName.Should().Be("gpt-4");
        sendData.Messages.Should().HaveCount(2); // System + User
        sendData.Messages[0].Role.Should().Be("system");
        sendData.Messages[1].Role.Should().Be("user");
        sendData.Messages[1].Content.ToString().Should().Be(text);
    }

    [Fact]
    public void GetSendDataAsync_WithCustomSettings_ShouldApplySettings()
    {
        // Arrange
        var api = new ChatLLMApi("test", "gpt-4", "prompt");
        var settings = new GenerateSettings(temperature: 0.8, maxTokens: 1000);

        // Act
        var sendData = api.GetSendDataAsync("test", settings);

        // Assert
        sendData.Temperature.Should().Be(0.8);
        sendData.MaxTokens.Should().Be(1000);
    }

    [Fact]
    public void GetSendDataAsync_WithNullContext_ShouldThrowArgumentException()
    {
        // Arrange
        var api = new ChatLLMApi("test", "gpt-4", "prompt");

        // Act
        var act = () => api.GetSendDataAsync(null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public async Task SendWithContextAsync_WithStreamId_ShouldPublishEverySseContentChunk()
    {
        var streamHandler = Substitute.For<IStreamHandler>();
        var publishedMessages = new List<string>();
        streamHandler.SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(callInfo =>
            {
                publishedMessages.Add(callInfo.ArgAt<string>(1));
                return Task.FromResult(true);
            });

        var webApi = Substitute.For<IWebAPIClient>();
        webApi.PostAsJsonAsync(Arg.Any<string>(), Arg.Any<SendDataLLM>(), Arg.Any<CancellationToken?>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                    data: {"choices":[{"delta":{"content":"Hel"}}]}

                    data: {"choices":[{"delta":{"content":"lo"}}]}

                    data: [DONE]
                    """)
            }));

        var api = new ChatLLMApi("test", "test-model", "prompt", streamHandler)
        {
            ApiUrl = "https://llm.example.test/v1/chat/completions"
        };
        typeof(ChatLLMApi)
            .GetField("_webApi", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(api, webApi);

        var response = await api.SendWithContextAsync(
            [LLMMessage.CreateMessage(Roles.User, "Hello")],
            new GenerateSettings(streamId: "stream-id"));

        response.Choices[0].Message.Content.ToString().Should().Be("Hello");
        publishedMessages.Should().Equal("Hel", "lo", "<<END_OF_MESSAGE>>");
    }
}

