using Application.Application.Validators;
using FluentAssertions;

namespace Application.Tests.ValidatorTests;

public class PercentageValidatorTests
{
    private readonly PercentageValidator _validator;

    public PercentageValidatorTests()
    {
        _validator = new PercentageValidator();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(60)]
    [InlineData(100)]
    public void Validate_ValidPercentageValue_ShouldNotHaveValidationError(double percentage)
    {
        // Act
        var result = _validator.Validate(percentage);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(-10)]
    [InlineData(0)]
    [InlineData(101)]
    [InlineData(120)]
    public void Validate_InvalidPercentageValue_ShouldHaveValidationError(double celsius)
    {
        // Act
        var result = _validator.Validate(celsius);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors
            .Should()
            .ContainSingle()
            .Which.ErrorMessage.Should()
            .Be("Invalid percentage value. The humidity percentage should be between 0 and 100.");
    }
}
