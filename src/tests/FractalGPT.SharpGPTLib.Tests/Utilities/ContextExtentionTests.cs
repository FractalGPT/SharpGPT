using FluentAssertions;
using FractalGPT.SharpGPTLib.Core.Models.Common.Messages;
using FractalGPT.SharpGPTLib.Utilities.Extensions;

namespace FractalGPT.SharpGPTLib.Tests.Utilities;

public class ContextExtentionTests
{
    [Fact]
    public void FixContext_WithValidAlternatingMessages_ShouldNotModify()
    {
        // Arrange
        var context = new List<LLMMessage>
        {
            LLMMessage.CreateMessage(Roles.System, "System prompt"),
            LLMMessage.CreateMessage(Roles.User, "User message"),
            LLMMessage.CreateMessage(Roles.Assistant, "Assistant response")
        };

        // Act
        var result = context.FixContext();

        // Assert
        result.Should().HaveCount(3);
        result[0].Role.Should().Be("system");
        result[1].Role.Should().Be("user");
        result[2].Role.Should().Be("assistant");
    }

    [Fact]
    public void FixContext_WithAssistantFirst_ShouldAddUserMessage()
    {
        // Arrange
        var context = new List<LLMMessage>
        {
            LLMMessage.CreateMessage(Roles.Assistant, "Assistant message")
        };

        // Act
        var result = context.FixContext();

        // Assert
        result.Should().HaveCount(2);
        result[0].Role.Should().Be("user");
        result[0].Content.ToString().Should().Be(" ");
        result[1].Role.Should().Be("assistant");
    }

    [Fact]
    public void FixContext_WithConsecutiveSameRoles_ShouldInsertEmptyMessage()
    {
        // Arrange
        var context = new List<LLMMessage>
        {
            LLMMessage.CreateMessage(Roles.User, "First user message"),
            LLMMessage.CreateMessage(Roles.User, "Second user message")
        };

        // Act
        var result = context.FixContext();

        // Assert
        result.Should().HaveCount(3);
        result[0].Role.Should().Be("user");
        result[1].Role.Should().Be("assistant");
        result[1].Content.ToString().Should().Be("");
        result[2].Role.Should().Be("user");
    }

    [Fact]
    public void FixContext_WithSystemThenAssistant_ShouldInsertUserMessage()
    {
        // Arrange
        var context = new List<LLMMessage>
        {
            LLMMessage.CreateMessage(Roles.System, "System prompt"),
            LLMMessage.CreateMessage(Roles.Assistant, "Assistant message")
        };

        // Act
        var result = context.FixContext();

        // Assert
        result.Should().HaveCount(3);
        result[0].Role.Should().Be("system");
        result[1].Role.Should().Be("user");
        result[1].Content.ToString().Should().Be("");
        result[2].Role.Should().Be("assistant");
    }

    [Fact]
    public void FixContext_WithComplexScenario_ShouldFixAllIssues()
    {
        // Arrange
        var context = new List<LLMMessage>
        {
            LLMMessage.CreateMessage(Roles.Assistant, "First assistant"),
            LLMMessage.CreateMessage(Roles.Assistant, "Second assistant"),
            LLMMessage.CreateMessage(Roles.User, "User message")
        };

        // Act
        var result = context.FixContext();

        // Assert
        result.Should().HaveCountGreaterThan(3);
        // First message should be preceded by user
        result[0].Role.Should().Be("user");
        // No consecutive same roles
        for (int i = 1; i < result.Count; i++)
        {
            result[i].Role.Should().NotBe(result[i - 1].Role);
        }
    }

    [Fact]
    public void FixContext_WithEmptyList_ShouldReturnEmptyList()
    {
        // Arrange
        var context = new List<LLMMessage>();

        // Act
        var result = context.FixContext();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void FixContext_WithSingleSystemMessage_ShouldNotModify()
    {
        // Arrange
        var context = new List<LLMMessage>
        {
            LLMMessage.CreateMessage(Roles.System, "System prompt")
        };

        // Act
        var result = context.FixContext();

        // Assert
        result.Should().HaveCount(1);
        result[0].Role.Should().Be("system");
    }

    [Fact]
    public void FixContext_WithSingleUserMessage_ShouldNotModify()
    {
        // Arrange
        var context = new List<LLMMessage>
        {
            LLMMessage.CreateMessage(Roles.User, "User message")
        };

        // Act
        var result = context.FixContext();

        // Assert
        result.Should().HaveCount(1);
        result[0].Role.Should().Be("user");
    }

    [Fact]
    public void FixContext_WithMultipleConsecutiveAssistantMessages_ShouldInsertUserMessages()
    {
        // Arrange
        var context = new List<LLMMessage>
        {
            LLMMessage.CreateMessage(Roles.User, "Question"),
            LLMMessage.CreateMessage(Roles.Assistant, "Answer 1"),
            LLMMessage.CreateMessage(Roles.Assistant, "Answer 2"),
            LLMMessage.CreateMessage(Roles.Assistant, "Answer 3")
        };

        // Act
        var result = context.FixContext();

        // Assert
        // Should have original 4 messages + 2 inserted user messages
        result.Should().HaveCount(6);
        // Check alternation
        for (int i = 1; i < result.Count; i++)
        {
            result[i].Role.Should().NotBe(result[i - 1].Role);
        }
    }
}
