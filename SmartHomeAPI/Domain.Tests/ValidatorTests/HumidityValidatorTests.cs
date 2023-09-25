using Domain.Domain.Exceptions;
using Domain.Domain.Validators;
using Domain.Tests.Builders;
using FluentAssertions;

namespace Domain.Tests.ValidatorTests;

public class HumidityValidatorTests
{
    private readonly HumidityValidator _validator;

    public HumidityValidatorTests()
    {
        _validator = new HumidityValidator();
    }

    [Fact]
    public void Valid_Humidity_Should_Be_True()
    {
        // arrange
        var humidity = new HumidityBuilder().Build();

        // act
        var validationResult = _validator.Validate(humidity).IsValid;

        // assert
        validationResult.Should().BeTrue();
    }

    [Fact]
    public void Invalid_Percentage_Should_Throw_DomainException()
    {
        // act and assert
        Assert.Throws<DomainException>(() =>
        {
            var humidity = new HumidityBuilder().WithPercentage(100.000001).Build();
        });
    }

    [Fact]
    public void Invalid_Date_Percentage_Should_Throw_DomainException()
    {
        // act and assert
        Assert.Throws<DomainException>(() =>
        {
            var humidity = new HumidityBuilder().WithDate(DateTime.Now.AddDays(1)).Build();
        });
    }
}
