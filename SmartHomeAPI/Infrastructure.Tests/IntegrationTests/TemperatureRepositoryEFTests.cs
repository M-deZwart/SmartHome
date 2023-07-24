using Domain.Domain.Entities;
using Domain.Tests.Builders;
using FluentAssertions;
using Infrastructure.Infrastructure;
using Infrastructure.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Infrastructure.Tests.IntegrationTests;

public class TemperatureRepositoryEFTests
{
    private DbContextOptions<SmartHomeContext> CreateNewInMemoryDatabase()
    {
        var optionsBuilder = new DbContextOptionsBuilder<SmartHomeContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        return optionsBuilder.Options;
    }

    private (SmartHomeContext context, ILogger<TemperatureRepositoryEF> logger) CreateTestContextAndLogger()
    {
        var options = CreateNewInMemoryDatabase();
        var context = new SmartHomeContext(options);
        var logger = new Mock<ILogger<TemperatureRepositoryEF>>().Object;
        return (context, logger);
    }

    [Fact]
    public async Task Create_Should_Add_Temperature_To_Context_And_SaveChanges()
    {
        // arrange
        var (context, logger) = CreateTestContextAndLogger();
        var temperatureRepository = new TemperatureRepositoryEF(context, logger);
        var temperature = new TemperatureBuilder().WithCelsius(21).Build();

        // act
        await temperatureRepository.Create(temperature);
        var savedTemperature = context.Temperatures.FirstOrDefault();

        // assert
        savedTemperature.Should().NotBeNull();
        savedTemperature?.Celsius.Should().Be(21);
    }

    [Fact]
    public async Task GetLatestTemperature_Should_Return_Latest_Temperature_When_Date_Exists()
    {
        // arrange
        var (context, logger) = CreateTestContextAndLogger();
        var temperatureRepository = new TemperatureRepositoryEF(context, logger);

        var mockData = new[]
        {
            new TemperatureBuilder().WithDate(DateTime.Now.AddHours(-2)).Build(),
            new TemperatureBuilder().WithDate(DateTime.Now.AddHours(-1)).Build(),
            new TemperatureBuilder().WithDate(DateTime.Now).Build()
        };
        context.Temperatures.AddRange(mockData);
        await context.SaveChangesAsync();

        // act
        var latestTemperature = await temperatureRepository.GetLatestTemperature();

        // assert
        latestTemperature.Should().NotBeNull();
        latestTemperature.Date.Should().Be(mockData[2].Date);
    }

    [Fact]
    public async Task GetByDateRange_Should_Return_Temperatures_Within_Given_Range()
    {
        // arrange
        var (context, logger) = CreateTestContextAndLogger();
        var temperatureRepository = new TemperatureRepositoryEF(context, logger);

        var startDate = DateTime.Now.AddHours(-5);
        var endDate = DateTime.Now.AddHours(-1);

        var mockData = new List<Temperature>()
        {
            new TemperatureBuilder().WithDate(DateTime.Now.AddHours(-10)),
            new TemperatureBuilder().WithDate(DateTime.Now.AddHours(-8)),
            new TemperatureBuilder().WithDate(startDate.AddMinutes(30)),
            new TemperatureBuilder().WithDate(endDate.AddMinutes(-30)),
            new TemperatureBuilder().WithDate(endDate),
            new TemperatureBuilder().WithDate(DateTime.Now.AddHours(-0.5))
        };

        context.Temperatures.AddRange(mockData);
        await context.SaveChangesAsync();

        // act
        var temperaturesInRange = await temperatureRepository.GetByDateRange(startDate, endDate);

        // assert
        temperaturesInRange.Should().NotBeNull();
        temperaturesInRange.Should().HaveCount(3);
        temperaturesInRange.All(h => h.Date >= startDate && h.Date <= endDate).Should().BeTrue();
    }
}
