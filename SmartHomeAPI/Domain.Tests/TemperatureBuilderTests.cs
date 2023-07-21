using Domain.Domain.Entities;
using Domain.Tests.Builders;
using FluentAssertions;

namespace Domain.Tests;

public class TemperatureBuilderTests
{
    [Fact]
    public void TestTemperatureBuilder()
    {
        // arrange
        Temperature temperature = new TemperatureBuilder()
            .WithCelsius(25.5)
            .WithDate(DateTime.Now.AddDays(+1))
            .Build();  
        
        // assert
        temperature.Id.Should().NotBeEmpty();
        temperature.Celsius.Should().Be(25.5);
        temperature.Date.Date.Should().Be(DateTime.Now.AddDays(+1).Date);
    }
}
