using FluentAssertions;
using FractalGPT.SharpGPTLib.Core.Models.Common.Messages.Content;

namespace FractalGPT.SharpGPTLib.Tests.Models;

public class ContentTests
{
    [Fact]
    public void TextContentItem_Constructor_ShouldSetText()
    {
        // Arrange
        var text = "Test text";

        // Act
        var item = new TextContentItem(text);

        // Assert
        item.Text.Should().Be(text);
        item.Type.Should().Be("text");
    }

    [Fact]
    public void ImageContent_WithUrl_ShouldSetImageUrl()
    {
        // Arrange
        var url = "https://example.com/image.png";

        // Act
        var imageContent = new ImageContent(url);

        // Assert
        imageContent.ImageUrl.Should().NotBeNull();
        imageContent.ImageUrl.Url.Should().Be(url);
        imageContent.Type.Should().Be("image_url");
    }

    [Fact]
    public void ImageContent_WithByteArray_ShouldCreateBase64String()
    {
        // Arrange
        var bytes = new byte[] { 1, 2, 3, 4, 5 };

        // Act
        var imageContent = new ImageContent(bytes);

        // Assert
        imageContent.ImageUrl.Should().NotBeNull();
        imageContent.ImageUrl.Url.Should().StartWith("data:image/png;base64,");
        imageContent.Type.Should().Be("image_url");
    }

    [Fact]
    public void ImageUrl_Constructor_ShouldSetUrl()
    {
        // Arrange
        var url = "https://example.com/test.jpg";

        // Act
        var imageUrl = new ImageUrl { Url = url };

        // Assert
        imageUrl.Url.Should().Be(url);
    }
}

