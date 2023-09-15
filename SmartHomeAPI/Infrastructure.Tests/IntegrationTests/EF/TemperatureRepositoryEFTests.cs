using Application.Application.Exceptions;
using Domain.Domain.Entities;
using Domain.Tests.Builders;
using FluentAssertions;
using Infrastructure.Infrastructure;
using Infrastructure.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests.IntegrationTests;

public class TemperatureRepositoryEFTests
{
    private readonly SmartHomeContext _context;
    private readonly TemperatureRepositoryEF _temperatureRepository;
    private readonly Sensor _sensor;
    private const string SENSOR_TITLE = "LivingRoom";

    public TemperatureRepositoryEFTests()
    {
        _context = CreateTestContext();
        _temperatureRepository = new TemperatureRepositoryEF(_context);
        _sensor = new Sensor(SENSOR_TITLE);
        _context.Sensors.Add(_sensor);
        _context.SaveChanges();
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
    public async Task Create_Should_Add_Temperature_To_Context_And_SaveChanges()
    {
        // arrange
        var temperature = new TemperatureBuilder().Build();

        // act
        await _temperatureRepository.Create(temperature, SENSOR_TITLE);
        var savedTemperature = _context.Temperatures.FirstOrDefault();

        // assert
        savedTemperature.Should().NotBeNull();
        savedTemperature?.Date.Should().BeCloseTo(temperature.Date, precision: TimeSpan.FromSeconds(1));
        savedTemperature?.Celsius.Should().Be(20);
    }

    [Fact]
    public async Task GetLatestTemperature_Should_Return_Latest_Temperature_When_Date_Exists()
    {
        // arrange
        var mockData = new[]
        {
            new TemperatureBuilder().WithDate(DateTime.Now.AddHours(-2)).Build(),
            new TemperatureBuilder().WithDate(DateTime.Now.AddHours(-1)).Build(),
            new TemperatureBuilder().WithDate(DateTime.Now).Build()
        };
        _sensor.Temperatures = mockData;
        _context.Temperatures.AddRange(mockData);
        await _context.SaveChangesAsync();

        // act
        var latestTemperature = await _temperatureRepository.GetLatestTemperature(SENSOR_TITLE);

        // assert
        latestTemperature.Should().NotBeNull();
        latestTemperature.Date.Should().Be(mockData[2].Date);
    }

    [Fact]
    public async Task GetByDateRange_Should_Return_Temperatures_WithinDateRange()
    {
        // arrange
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
        _sensor.Temperatures = mockData;
        _context.Temperatures.AddRange(mockData);
        await _context.SaveChangesAsync();

        // act
        var temperaturesInRange = await _temperatureRepository.GetByDateRange(startDate, endDate, SENSOR_TITLE);

        // assert
        temperaturesInRange.Should().NotBeNull();
        temperaturesInRange.Should().HaveCount(3);
        temperaturesInRange.All(h => h.Date >= startDate && h.Date <= endDate).Should().BeTrue();
    }

    [Fact]
    public async Task GetLatestTemperature_Should_Throw_NotFoundException_When_No_Temperature_Exists()
    {
        // act
        Func<Task> act = async () => await _temperatureRepository.GetLatestTemperature(SENSOR_TITLE);

        // assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

}
