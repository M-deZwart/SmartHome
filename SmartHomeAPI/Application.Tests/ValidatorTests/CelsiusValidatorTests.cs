using Application.Application.Validators;
using FluentAssertions;

namespace Application.Tests.ValidatorTests;

public class CelsiusValidatorTests
{
    private readonly CelsiusValidator _validator;

    public CelsiusValidatorTests()
    {
        _validator = new CelsiusValidator();
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(30)]
    [InlineData(40)]
    public void Validate_ValidCelsiusValue_ShouldNotHaveValidationError(double celsius)
    {
        // Act
        var result = _validator.Validate(celsius);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(5)]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(-10)]
    public void Validate_InvalidCelsiusValue_ShouldHaveValidationError(double celsius)
    {
        // Act
        var result = _validator.Validate(celsius);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors
            .Should()
            .ContainSingle()
            .Which.ErrorMessage.Should()
            .Be(
                "Invalid Celsius value. The temperature in Celsius should be between 10 and 40 degrees."
            );
    }
}
