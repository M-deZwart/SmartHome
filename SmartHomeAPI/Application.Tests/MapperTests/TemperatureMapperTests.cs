using Smarthome.Application.DTOs;
using Domain.Tests.Builders;
using FluentAssertions;

namespace Application.Tests.MapperTests;

public class TemperatureMapperTests
{
    [Fact]
    public void MapToDTO_Should_Map_To_TemperatureDTO()
    {
        // Arrange
        var temperature = new TemperatureBuilder().WithCelsius(23.5).WithDate(DateTime.Now).Build();

        // Act
        var result = TemperatureDTO.FromDomain(temperature);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<TemperatureDTO>();
        result.Celsius.Should().Be(temperature.Celsius);
        result.Date.Should().BeCloseTo(temperature.Date, precision: TimeSpan.FromSeconds(1));
    }
}
