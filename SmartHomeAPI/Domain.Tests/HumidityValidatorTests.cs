using Domain.Domain.Validators;
using Domain.Tests.Builders;
using FluentAssertions;

namespace Domain.Tests;

public class HumidityValidatorTests : IClassFixture<HumidityValidator>
{
    private readonly HumidityValidator _validator;

    public HumidityValidatorTests(HumidityValidator validator)
    {
        _validator = validator;
    }

    [Fact]
    public void TestValidHumidity_ShouldBeTrue()
    {
        // arrange
        var humidity = new HumidityBuilder().Build();

        // act
        var validationResult = _validator.Validate(humidity).IsValid;

        // assert
        validationResult.Should().BeTrue();
    }

    [Fact]
    public void TestInvalidHumidity_ShouldBeFalse()
    {
        // arrange
        var humidity = new HumidityBuilder()
            .WithPercentage(-0.000001)
            .Build();

        // act
        var validationResult = _validator.Validate(humidity).IsValid;

        // assert
        validationResult.Should().BeFalse();
    }

    [Fact]
    public void TestInvalidHumidityMessage()
    {
        // act
        var humidity = new HumidityBuilder()
            .WithPercentage(100.000001)
            .Build();

        // act
        var validationResult = _validator.Validate(humidity);

        // assert
        validationResult.Errors.Should().ContainSingle();
        validationResult.Errors[0].ErrorMessage.Should().Be("Invalid humidity value. The humidity percentage should be between 0 and 100.");
    }
}
