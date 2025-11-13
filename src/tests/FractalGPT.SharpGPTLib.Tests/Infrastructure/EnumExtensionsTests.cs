using FluentAssertions;
using FractalGPT.SharpGPTLib.Infrastructure.Extensions;
using System.ComponentModel;

namespace FractalGPT.SharpGPTLib.Tests.Infrastructure;

public class EnumExtensionsTests
{
    private enum TestEnum
    {
        [Description("First Value")]
        Value1,
        
        [Description("Second Value")]
        Value2,
        
        ValueWithoutDescription
    }

    [Fact]
    public void GetDescription_WithDescriptionAttribute_ShouldReturnDescription()
    {
        // Arrange
        var enumValue = TestEnum.Value1;

        // Act
        var description = enumValue.GetDescription();

        // Assert
        description.Should().Be("First Value");
    }

    [Fact]
    public void GetDescription_WithoutDescriptionAttribute_ShouldReturnEnumName()
    {
        // Arrange
        var enumValue = TestEnum.ValueWithoutDescription;

        // Act
        var description = enumValue.GetDescription();

        // Assert
        description.Should().Be("ValueWithoutDescription");
    }

    [Fact]
    public void GetDescription_WithMultipleValues_ShouldReturnCorrectDescriptions()
    {
        // Assert
        TestEnum.Value1.GetDescription().Should().Be("First Value");
        TestEnum.Value2.GetDescription().Should().Be("Second Value");
        TestEnum.ValueWithoutDescription.GetDescription().Should().Be("ValueWithoutDescription");
    }
}

