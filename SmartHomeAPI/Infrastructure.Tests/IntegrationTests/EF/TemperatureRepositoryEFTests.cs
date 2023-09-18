using Application.Application.Exceptions;
using Domain.Domain.Entities;
using Domain.Tests.Builders;
using FluentAssertions;
using Infrastructure.Infrastructure.Repositories;

namespace Infrastructure.Tests.IntegrationTests;

public class TemperatureRepositoryEFTests : CommonTestBase
{
    private readonly TemperatureRepositoryEF _temperatureRepository;

    public TemperatureRepositoryEFTests()
    {
        _temperatureRepository = new TemperatureRepositoryEF(Context);       
    }

    [Fact]
    public async Task Create_Should_Add_Temperature_To_Context_And_SaveChanges()
    {
        // arrange
        var temperature = new TemperatureBuilder().Build();

        // act
        await _temperatureRepository.Create(temperature, SENSOR_TITLE);
        var savedTemperature = Context.Temperatures.FirstOrDefault();

        // assert
        savedTemperature.Should().NotBeNull();
        savedTemperature?.Date.Should().BeCloseTo(temperature.Date, precision: TimeSpan.FromSeconds(1));
        savedTemperature?.Celsius.Should().Be(20);
    }

    [Fact]
    public async Task GetLatestTemperature_Should_Return_Latest_Temperature_When_Date_Exists()
    {
        // arrange
        var mockData = new List<Temperature>()
        {
            new TemperatureBuilder().WithDate(DateTime.Now.AddHours(-2)).Build(),
            new TemperatureBuilder().WithDate(DateTime.Now.AddHours(-1)).Build(),
            new TemperatureBuilder().WithDate(DateTime.Now).Build()
        };
        foreach (var temperature in mockData)
        {
            await _temperatureRepository.Create(temperature, SENSOR_TITLE);
        }

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
        foreach (var temperature in mockData)
        {
            await _temperatureRepository.Create(temperature, SENSOR_TITLE);
        }

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
