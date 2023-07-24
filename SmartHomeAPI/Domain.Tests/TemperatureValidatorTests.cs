using Domain.Domain.Validators;
using Domain.Tests.Builders;
using FluentAssertions;

namespace Domain.Tests;

public class TemperatureValidatorTests : IClassFixture<TemperatureValidator>
{
    private readonly TemperatureValidator _validator;

    public TemperatureValidatorTests(TemperatureValidator validator)
    {
        _validator = validator;
    }

    [Fact]
    public void Valid_Temperature_Should_Be_True()
    {
        // arrange
        var temperature = new TemperatureBuilder().Build();

        // act
        var validationResult = _validator.Validate(temperature).IsValid;

        // assert
        validationResult.Should().BeTrue();
    }

    [Fact]
    public void Invalid_Temperature_Should_Be_False()
    {
        // arrange
        var temperature = new TemperatureBuilder()
            .WithCelsius(9.99)
            .Build();

        // act
        var validationResult = _validator.Validate(temperature).IsValid;

        // assert
        validationResult.Should().BeFalse();
    }

    [Fact]
    public void Invalid_Temperature_Should_Return_Error_Message()
    {
        // arrange
        var temperature = new TemperatureBuilder()
            .WithCelsius(40.00001)
            .Build();

        // act
        var validationResult = _validator.Validate(temperature);

        // assert
        validationResult.Errors.Should().ContainSingle();
        validationResult.Errors[0].ErrorMessage.Should().Be("Invalid temperature value. The temperature in Celsius should be between 10 and 40 degrees.");
    }

}
