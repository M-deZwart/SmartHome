using Application.Application.Exceptions;
using Application.Application.Services;
using Domain.Domain.Entities;
using Domain.Tests.Builders;
using FluentAssertions;
using Infrastructure.Infrastructure;
using Infrastructure.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests.IntegrationTests;

public class HumidityRepositoryEFTests
{
    private readonly SmartHomeContext _context;
    private readonly HumidityRepositoryEF _humidityRepository;

    public HumidityRepositoryEFTests()
    {
        _context = CreateTestContext();
        _humidityRepository = new HumidityRepositoryEF(_context);
    }

    private DbContextOptions<SmartHomeContext> CreateNewInMemoryDatabase()
    {
        var optionsBuilder = new DbContextOptionsBuilder<SmartHomeContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        return optionsBuilder.Options;
    }

    private SmartHomeContext CreateTestContext()
    {
        var options = CreateNewInMemoryDatabase();
        var context = new SmartHomeContext(options);
        return context;
    }

    [Fact]
    public async Task Create_Should_Add_Humidity_To_Context_And_SaveChanges()
    {
        // arrange
        var humidity = new HumidityBuilder().WithPercentage(25).Build();

        // act
        await _humidityRepository.Create(humidity);
        var savedHumidity = _context.Humidities.FirstOrDefault();

        // assert
        savedHumidity.Should().NotBeNull();
        savedHumidity?.Percentage.Should().Be(25);
    }

    [Fact]
    public async Task GetLatestHumidity_Should_Return_Latest_Humidity_When_Date_Exists()
    {
        // arrange
        var mockData = new[]
        {
            new HumidityBuilder().WithDate(DateTime.Now.AddHours(-2)).Build(),
            new HumidityBuilder().WithDate(DateTime.Now.AddHours(-1)).Build(),
            new HumidityBuilder().WithDate(DateTime.Now).Build()
        };
        _context.Humidities.AddRange(mockData);
        await _context.SaveChangesAsync();

        // act
        var latestHumidity = await _humidityRepository.GetLatestHumidity();

        // assert
        latestHumidity.Should().NotBeNull();
        latestHumidity.Date.Should().Be(mockData[2].Date);
    }

    [Fact]
    public async Task GetLatestHumidity_Should_Throw_NotFoundException_When_No_Humidity_Exists()
    {
        // act and assert
        await Assert.ThrowsAsync<NotFoundException>(_humidityRepository.GetLatestHumidity);
    }

    [Fact]
    public async Task GetByDateRange_Should_Return_Humidities_WithinDateRange()
    {
        // arrange
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

        _context.Humidities.AddRange(mockData);
        await _context.SaveChangesAsync();

        // act
        var humiditiesInRange = await _humidityRepository.GetByDateRange(startDate, endDate);

        // assert
        humiditiesInRange.Should().NotBeNull();
        humiditiesInRange.Should().HaveCount(3);
        humiditiesInRange.All(h => h.Date >= startDate && h.Date <= endDate).Should().BeTrue();
    }
}
