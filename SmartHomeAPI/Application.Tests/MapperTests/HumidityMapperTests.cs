using Smarthome.Application.ApiMappers;
using Smarthome.Application.DTOs;
using Domain.Tests.Builders;
using FluentAssertions;

namespace Application.Tests.MapperTests;

public class HumidityMapperTests
{
    [Fact]
    public void MapToDTO_Should_Map_To_HumidityDTO()
    {
        // Arrange
        var mapper = new HumidityMapper();
        var humidity = new HumidityBuilder().WithPercentage(75.5).WithDate(DateTime.Now).Build();

        // Act
        var result = mapper.MapToDTO(humidity);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<HumidityDTO>();
        result.Percentage.Should().Be(humidity.Percentage);
        result.Date.Should().BeCloseTo(humidity.Date, precision: TimeSpan.FromSeconds(1));
    }
}
