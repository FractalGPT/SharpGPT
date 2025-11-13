using FluentAssertions;
using FractalGPT.SharpGPTLib.Core.Models.Common.Messages;
using FractalGPT.SharpGPTLib.Core.Models.Common.Messages.Content;

namespace FractalGPT.SharpGPTLib.Tests.Models;

public class LLMMessageTests
{
    [Fact]
    public void Constructor_WithRoleAndString_ShouldCreateMessage()
    {
        // Arrange
        var role = "user";
        var content = "Hello, world!";

        // Act
        var message = new LLMMessage(role, content);

        // Assert
        message.Role.Should().Be(role);
        message.Content.Should().Be(content);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidRole_ShouldThrowArgumentException(string invalidRole)
    {
        // Act
        var act = () => new LLMMessage(invalidRole, "content");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("role");
    }

    [Fact]
    public void CreateMessage_WithSystemRole_ShouldCreateSystemMessage()
    {
        // Act
        var message = LLMMessage.CreateMessage(Roles.System, "You are a helpful assistant");

        // Assert
        message.Role.Should().Be("system");
        message.Content.Should().Be("You are a helpful assistant");
    }

    [Fact]
    public void CreateMessage_WithUserRole_ShouldCreateUserMessage()
    {
        // Act
        var message = LLMMessage.CreateMessage(Roles.User, "Hello");

        // Assert
        message.Role.Should().Be("user");
        message.Content.Should().Be("Hello");
    }

    [Fact]
    public void CreateMessage_WithAssistantRole_ShouldCreateAssistantMessage()
    {
        // Act
        var message = LLMMessage.CreateMessage(Roles.Assistant, "Hi there!");

        // Assert
        message.Role.Should().Be("assistant");
        message.Content.Should().Be("Hi there!");
    }

    [Fact]
    public void Constructor_WithMessageContent_ShouldSetContent()
    {
        // Arrange
        var messageContent = new MessageContent("Test content");

        // Act
        var message = new LLMMessage("user", messageContent);

        // Assert
        message.Role.Should().Be("user");
        message.Content.Should().BeOfType<MessageContent>();
    }

    [Fact]
    public void DeepClone_ShouldCreateExactCopy()
    {
        // Arrange
        var original = LLMMessage.CreateMessage(Roles.User, "Original content");

        // Act
        var clone = original.DeepClone();

        // Assert
        clone.Should().NotBeSameAs(original);
        clone.Role.Should().Be(original.Role);
        clone.Content.Should().Be(original.Content);
    }

    [Fact]
    public void DefaultConstructor_ShouldCreateEmptyMessage()
    {
        // Act
        var message = new LLMMessage();

        // Assert
        message.Role.Should().Be(string.Empty);
        message.Content.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithObjectContent_StringType_ShouldSetStringContent()
    {
        // Arrange
        object content = "Test string";

        // Act
        var message = new LLMMessage("user", content);

        // Assert
        message.Content.Should().Be("Test string");
    }

    [Fact]
    public void Constructor_WithObjectContent_MessageContentType_ShouldSetMessageContent()
    {
        // Arrange
        object content = new MessageContent("Test");

        // Act
        var message = new LLMMessage("user", content);

        // Assert
        message.Content.Should().BeOfType<MessageContent>();
    }

    [Fact]
    public void Constructor_WithObjectContent_InvalidType_ShouldThrowException()
    {
        // Arrange
        object content = 123; // Invalid type

        // Act
        var act = () => new LLMMessage("user", content);

        // Assert
        act.Should().Throw<Exception>()
            .WithMessage("*не поддерживаемый тип*");
    }

    [Fact]
    public void Message_CanHaveRefusal()
    {
        // Arrange
        var message = LLMMessage.CreateMessage(Roles.Assistant, "Content");

        // Act
        message.Refusal = "I cannot help with that";

        // Assert
        message.Refusal.Should().Be("I cannot help with that");
    }

    [Fact]
    public void Message_CanHaveReasoning()
    {
        // Arrange
        var message = LLMMessage.CreateMessage(Roles.Assistant, "Answer");

        // Act
        message.Reasoning = "This is my reasoning";

        // Assert
        message.Reasoning.Should().Be("This is my reasoning");
    }
}

