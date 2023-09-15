using Application.Application.Exceptions;
using Domain.Domain.Entities;
using Domain.Tests.Builders;
using FluentAssertions;
using Infrastructure.Infrastructure;
using Infrastructure.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests.IntegrationTests;

public class HumidityRepositoryEFTests : CommonTestBase
{
    private readonly HumidityRepositoryEF _humidityRepository;

    public HumidityRepositoryEFTests()
    {
        _humidityRepository = new HumidityRepositoryEF(Context);
    }

    [Fact]
    public async Task Create_Should_Add_Humidity_To_Context_And_SaveChanges()
    {
        // arrange
        var humidity = new HumidityBuilder().WithPercentage(25).Build();

        // act
        await _humidityRepository.Create(humidity, SENSOR_TITLE);
        var savedHumidity = Context.Humidities.FirstOrDefault();

        // assert
        savedHumidity.Should().NotBeNull();
        savedHumidity?.Date.Should().BeCloseTo(humidity.Date, precision: TimeSpan.FromSeconds(1));
        savedHumidity?.Percentage.Should().Be(25);
    }

    [Fact]
    public async Task GetLatestHumidity_Should_Return_Latest_Humidity_When_Date_Exists()
    {
        // arrange
        var mockData = new List<Humidity>()
        {
            new HumidityBuilder().WithDate(DateTime.Now.AddHours(-2)).Build(),
            new HumidityBuilder().WithDate(DateTime.Now.AddHours(-1)).Build(),
            new HumidityBuilder().WithDate(DateTime.Now).Build()
        };
        foreach (var humidity in mockData)
        {
            await _humidityRepository.Create(humidity, SENSOR_TITLE);
        }

        // act
        var latestHumidity = await _humidityRepository.GetLatestHumidity(SENSOR_TITLE);

        // assert
        latestHumidity.Should().NotBeNull();
        latestHumidity.Date.Should().Be(mockData[2].Date);
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

        foreach (var humidity in mockData)
        {
            await _humidityRepository.Create(humidity, SENSOR_TITLE);
        }

        // act
        var humiditiesInRange = await _humidityRepository.GetByDateRange(startDate, endDate, SENSOR_TITLE);

        // assert
        humiditiesInRange.Should().NotBeNull();
        humiditiesInRange.Should().HaveCount(3);
        humiditiesInRange.All(h => h.Date >= startDate && h.Date <= endDate).Should().BeTrue();
    }

    [Fact]
    public async Task GetLatestHumidity_Should_Throw_NotFoundException_When_No_Humidity_Exists()
    {
        // act
        Func<Task> act = async () => await _humidityRepository.GetLatestHumidity(SENSOR_TITLE);

        // assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

}
