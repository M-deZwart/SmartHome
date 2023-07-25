using Domain.Domain.Entities;
using Domain.Tests.Builders;
using FluentAssertions;
using Infrastructure.Infrastructure;
using Infrastructure.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Infrastructure.Tests.IntegrationTests;

public class HumidityRepositoryEFTests
{
    private DbContextOptions<SmartHomeContext> CreateNewInMemoryDatabase()
    {
        var optionsBuilder = new DbContextOptionsBuilder<SmartHomeContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        return optionsBuilder.Options;
    }

    private (SmartHomeContext context, ILogger<HumidityRepositoryEF> logger) CreateTestContextAndLogger()
    {
        var options = CreateNewInMemoryDatabase();
        var context = new SmartHomeContext(options);
        var logger = new Mock<ILogger<HumidityRepositoryEF>>().Object;
        return (context, logger);
    }

    [Fact]
    public async Task Create_Should_Add_Humidity_To_Context_And_SaveChanges()
    {
        // arrange
        var (context, logger) = CreateTestContextAndLogger();
        var humidityRepository = new HumidityRepositoryEF(context, logger);
        var humidity = new HumidityBuilder().WithPercentage(25).Build();

        // act
        await humidityRepository.Create(humidity);
        var savedHumidity = context.Humidities.FirstOrDefault();

        // assert
        savedHumidity.Should().NotBeNull();
        savedHumidity?.Percentage.Should().Be(25);
    }

    [Fact]
    public async Task GetLatestHumidity_Should_Return_Latest_Humidity_When_Date_Exists()
    {
        // arrange
        var (context, logger) = CreateTestContextAndLogger();
        var humidityRepository = new HumidityRepositoryEF(context, logger);

        var mockData = new[]
        {
            new HumidityBuilder().WithDate(DateTime.Now.AddHours(-2)).Build(),
            new HumidityBuilder().WithDate(DateTime.Now.AddHours(-1)).Build(),
            new HumidityBuilder().WithDate(DateTime.Now).Build()
        };
        context.Humidities.AddRange(mockData);
        await context.SaveChangesAsync();

        // act
        var latestHumidity = await humidityRepository.GetLatestHumidity();

        // assert
        latestHumidity.Should().NotBeNull();
        latestHumidity.Date.Should().Be(mockData[2].Date);
    }

    [Fact]
    public async Task GetByDateRange_Should_Return_Humidities_WithinDateRange()
    {
        // arrange
        var (context, logger) = CreateTestContextAndLogger();
        var humidityRepository = new HumidityRepositoryEF(context, logger);

        var startDate = DateTime.Now.AddHours(-5);
        var endDate = DateTime.Now.AddHours(-1);

        var mockData = new List<Humidity>()
        {
            new HumidityBuilder().WithDate(DateTime.Now.AddHours(-10)),
            new HumidityBuilder().WithDate(DateTime.Now.AddHours(-8)),
            new HumidityBuilder().WithDate(startDate.AddMinutes(30)),
            new HumidityBuilder().WithDate(endDate.AddMinutes(-30)),
            new HumidityBuilder().WithDate(endDate),
            new HumidityBuilder().WithDate(DateTime.Now.AddHours(-0.5))
        };

        context.Humidities.AddRange(mockData);
        await context.SaveChangesAsync();

        // act
        var humiditiesInRange = await humidityRepository.GetByDateRange(startDate, endDate);

        // assert
        humiditiesInRange.Should().NotBeNull();
        humiditiesInRange.Should().HaveCount(3);
        humiditiesInRange.All(h => h.Date >= startDate && h.Date <= endDate).Should().BeTrue();
    }

    [Fact]
    public async Task GetLatestHumidity_Should_Throw_InvalidOperationException_If_Humidity_NotFound()
    {
        // arrange
        var (context, logger) = CreateTestContextAndLogger();
        var humidityRepository = new HumidityRepositoryEF(context, logger);

        // act
        var act = humidityRepository.GetLatestHumidity;

        // assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Failed to get current humidity: Humidity was not found");
    }

    [Fact]
    public async Task Create_NullValue_ThrowsInvalidOperationException()
    {
        // arrange
        var (context, logger) = CreateTestContextAndLogger();
        var humidityRepository = new HumidityRepositoryEF(context, logger);

        // act & assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => humidityRepository.Create(null));
    }
}
