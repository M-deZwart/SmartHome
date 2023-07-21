using Domain.Domain.Entities;
using Domain.Tests.Builders;
using FluentAssertions;

namespace Domain.Tests;

public class HumidityBuilderTests
{
    [Fact]
    public void TestHumidityBuilder()
    {
        // arrange
        Humidity humidity = new HumidityBuilder()
            .WithId(Guid.NewGuid())
            .WithPercentage(80.5)
            .WithDate(DateTime.Now.AddDays(-1));

        // assert
        humidity.Id.Should().NotBeEmpty();
        humidity.Percentage.Should().Be(80.5);
        humidity.Date.Date.Should().Be(DateTime.Now.AddDays(-1).Date);
    }
}
