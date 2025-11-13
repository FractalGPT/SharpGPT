using FluentAssertions;
using FractalGPT.SharpGPTLib.Core.Models.Common.Messages.Content;

namespace FractalGPT.SharpGPTLib.Tests.Models;

public class MessageContentTests
{
    [Fact]
    public void DefaultConstructor_ShouldCreateEmptyMessageContent()
    {
        // Act
        var content = new MessageContent();

        // Assert
        content.Should().NotBeNull();
        content.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithString_ShouldCreateTextContent()
    {
        // Arrange
        var text = "Hello, world!";

        // Act
        var content = new MessageContent(text);

        // Assert
        content.Should().HaveCount(1);
        content[0].Should().BeOfType<TextContentItem>();
        ((TextContentItem)content[0]).Text.Should().Be(text);
    }

    [Fact]
    public void AddText_ShouldAddTextContentItem()
    {
        // Arrange
        var content = new MessageContent();
        var text = "Test text";

        // Act
        content.AddText(text);

        // Assert
        content.Should().HaveCount(1);
        content[0].Should().BeOfType<TextContentItem>();
        ((TextContentItem)content[0]).Text.Should().Be(text);
    }

    [Fact]
    public void AddImage_WithUrl_ShouldAddImageContent()
    {
        // Arrange
        var content = new MessageContent();
        var imageUrl = "https://example.com/image.png";

        // Act
        content.AddImage(imageUrl);

        // Assert
        content.Should().HaveCount(1);
        content[0].Should().BeOfType<ImageContent>();
    }

    [Fact]
    public void AddImage_WithByteArray_ShouldAddImageContent()
    {
        // Arrange
        var content = new MessageContent();
        var imageBytes = new byte[] { 1, 2, 3, 4, 5 };

        // Act
        content.AddImage(imageBytes);

        // Assert
        content.Should().HaveCount(1);
        content[0].Should().BeOfType<ImageContent>();
    }

    [Fact]
    public void ToString_WithTextContent_ShouldReturnText()
    {
        // Arrange
        var text = "Test content";
        var content = new MessageContent(text);

        // Act
        var result = content.ToString();

        // Assert
        result.Should().Be(text);
    }

    [Fact]
    public void ToString_WithEmptyContent_ShouldReturnEmptyString()
    {
        // Arrange
        var content = new MessageContent();

        // Act
        var result = content.ToString();

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void ToString_WithImageOnly_ShouldReturnEmptyString()
    {
        // Arrange
        var content = new MessageContent();
        content.AddImage("https://example.com/image.png");

        // Act
        var result = content.ToString();

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void MessageContent_CanContainMultipleItems()
    {
        // Arrange
        var content = new MessageContent();

        // Act
        content.AddText("First text");
        content.AddImage("https://example.com/image.png");
        content.AddText("Second text");

        // Assert
        content.Should().HaveCount(3);
        content[0].Should().BeOfType<TextContentItem>();
        content[1].Should().BeOfType<ImageContent>();
        content[2].Should().BeOfType<TextContentItem>();
    }
}

