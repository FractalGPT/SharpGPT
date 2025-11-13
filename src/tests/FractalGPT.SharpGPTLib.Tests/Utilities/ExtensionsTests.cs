using FluentAssertions;
using FractalGPT.SharpGPTLib.Core.Models.Common.Messages;
using FractalGPT.SharpGPTLib.Utilities.Extensions;
using Xunit;

namespace FractalGPT.SharpGPTLib.Tests.Utilities;

public class ExtensionsTests
{
    [Fact]
    public void FixContext_ShouldHandleEmptyList()
    {
        // Arrange
        var context = new List<LLMMessage>();

        // Act
        var result = context.FixContext();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void FixContext_ShouldEnsureAlternatingRoles()
    {
        // Arrange
        var context = new List<LLMMessage>
        {
            LLMMessage.CreateMessage(Roles.User, "Hello"),
            LLMMessage.CreateMessage(Roles.User, "World")
        };

        // Act
        var result = context.FixContext();

        // Assert
        result.Should().HaveCountGreaterThan(2);
    }
}

