using Domain.Domain.Exceptions;
using Domain.Domain.Validators;
using Domain.Tests.Builders;
using FluentAssertions;

namespace Domain.Tests.ValidatorTests;

public class TemperatureValidatorTests
{
    private readonly TemperatureValidator _validator;

    public TemperatureValidatorTests()
    {
        _validator = new TemperatureValidator();
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
    public void Invalid_Celsius_Should_Throw_DomainException()
    {
        // act and assert
        Assert.Throws<DomainException>(() =>
        {
            var temperature = new TemperatureBuilder().WithCelsius(9.99).Build();
        });
    }

    [Fact]
    public void Invalid_Date_Should_Throw_DomainException()
    {
        // act and assert
        Assert.Throws<DomainException>(() =>
        {
            var temperature = new TemperatureBuilder().WithDate(DateTime.Now.AddDays(2)).Build();
        });
    }
}
