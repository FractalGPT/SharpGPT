using FluentAssertions;
using FractalGPT.SharpGPTLib.API.LLMAPI;
using FractalGPT.SharpGPTLib.Core.Models.Common.Messages;
using FractalGPT.SharpGPTLib.Core.Models.Common.Requests;

namespace FractalGPT.SharpGPTLib.Tests.Models;

public class SendDataLLMTests
{
    [Fact]
    public void Constructor_WithValidModelName_ShouldInitializeProperties()
    {
        // Arrange
        var modelName = "gpt-4";
        var settings = new GenerateSettings(temperature: 0.7, maxTokens: 1000);

        // Act
        var sendData = new SendDataLLM(modelName, settings);

        // Assert
        sendData.ModelName.Should().Be(modelName);
        sendData.Temperature.Should().Be(0.7);
        sendData.MaxTokens.Should().Be(1000);
        sendData.Messages.Should().NotBeNull().And.BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidModelName_ShouldThrowArgumentNullException(string invalidModelName)
    {
        // Act
        var act = () => new SendDataLLM(invalidModelName);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("modelName");
    }

    [Fact]
    public void Constructor_WithNullSettings_ShouldUseDefaultSettings()
    {
        // Arrange
        var modelName = "gpt-4";

        // Act
        var sendData = new SendDataLLM(modelName, null);

        // Assert
        sendData.ModelName.Should().Be(modelName);
        sendData.Messages.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void SetMessages_WithValidMessages_ShouldSetMessages()
    {
        // Arrange
        var sendData = new SendDataLLM("gpt-4");
        var messages = new List<LLMMessage>
        {
            LLMMessage.CreateMessage(Roles.System, "You are a helpful assistant"),
            LLMMessage.CreateMessage(Roles.User, "Hello")
        };

        // Act
        sendData.SetMessages(messages);

        // Assert
        sendData.Messages.Should().HaveCount(2);
        sendData.Messages[0].Role.Should().Be("system");
        sendData.Messages[1].Role.Should().Be("user");
    }

    [Fact]
    public void GetJson_ShouldReturnValidJsonString()
    {
        // Arrange
        var sendData = new SendDataLLM("gpt-4", new GenerateSettings(temperature: 0.5));
        sendData.SetMessages(new List<LLMMessage>
        {
            LLMMessage.CreateMessage(Roles.User, "Test")
        });

        // Act
        var json = sendData.GetJson();

        // Assert
        json.Should().NotBeNullOrEmpty();
        json.Should().Contain("\"model\":\"gpt-4\"");
        json.Should().Contain("\"temperature\":0.5");
    }

    [Fact]
    public void GetJson_WithWriteIndented_ShouldReturnFormattedJson()
    {
        // Arrange
        var sendData = new SendDataLLM("gpt-4");
        sendData.SetMessages(new List<LLMMessage>
        {
            LLMMessage.CreateMessage(Roles.User, "Test")
        });

        // Act
        var json = sendData.GetJson(writeIndented: true);

        // Assert
        json.Should().NotBeNullOrEmpty();
        json.Should().Contain("\n");  // Should have line breaks
        json.Should().Contain("  ");   // Should have indentation
    }

    [Fact]
    public void GetJson_WithNullValues_ShouldOmitNullProperties()
    {
        // Arrange
        var settings = new GenerateSettings(temperature: 0.5);
        var sendData = new SendDataLLM("gpt-4", settings);

        // Act
        var json = sendData.GetJson();

        // Assert
        json.Should().NotContain("\"topK\"");  // null values should be omitted
        json.Should().Contain("\"temperature\"");  // non-null values should be present
    }
}

