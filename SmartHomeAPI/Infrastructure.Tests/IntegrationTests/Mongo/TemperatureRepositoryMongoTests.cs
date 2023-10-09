using Smarthome.Application.Exceptions;
using Smarthome.Domain.Contracts;
using Smarthome.Domain.Entities;
using Domain.Tests.Builders;
using FluentAssertions;
using Infrastructure.Tests.IntegrationTests.TestInfra;
using MongoDB.Driver;
using Smarthome.Infrastructure.Repositories;

namespace Infrastructure.Tests.IntegrationTests.Mongo;

[Collection("MongoCollection")]
public class TemperatureRepositoryMongoTests : IDisposable
{
    private readonly IMongoDatabase _database;
    private readonly MongoClient _mongoClient;
    private readonly string _databaseName;

    private readonly IMongoCollection<Temperature> _temperatureCollection;
    private readonly IMongoCollection<Sensor> _sensorCollection;

    private readonly ITemperatureRepository _temperatureRepository;
    private readonly Sensor _sensor;

    private const string SENSOR_TITLE = "LivingRoom";

    public TemperatureRepositoryMongoTests(MongoFixture mongoFixture)
    {
        _mongoClient = new MongoClient(mongoFixture.ConnectionString);
        _databaseName = mongoFixture.Database;
        _database = _mongoClient.GetDatabase(_databaseName);

        _temperatureRepository = new TemperatureRepositoryMongo(_database);
        _temperatureCollection = _database.GetCollection<Temperature>("Temperature");

        _sensor = new Sensor(SENSOR_TITLE);
        _sensorCollection = _database.GetCollection<Sensor>("Sensor");
        _sensorCollection.InsertOne(_sensor);
    }

    [Fact]
    public async Task Create_Should_Add_Temperature_To_Database()
    {
        // arrange
        var temperature = new TemperatureBuilder().Build();

        // act
        await _temperatureRepository.Create(temperature, SENSOR_TITLE);

        // assert
        var filter = Builders<Temperature>.Filter.Eq("Celsius", temperature.Celsius);
        var result = await _temperatureCollection.Find(filter).FirstOrDefaultAsync();

        result.Should().NotBeNull();
        result.Date
            .Should()
            .BeCloseTo(temperature.Date.ToUniversalTime(), precision: TimeSpan.FromSeconds(1));
        result.Should().BeEquivalentTo(temperature, options => options.Excluding(x => x.Date));
    }

    [Fact]
    public async Task GetByDateRange_Should_Return_TemperatureList_WithinDateRange()
    {
        // arrange
        var startDate = DateTime.UtcNow.AddHours(-24);
        var endDate = DateTime.UtcNow;

        var mockData = new List<Temperature>
        {
            new TemperatureBuilder().WithDate(startDate.AddMinutes(30).ToUniversalTime()).Build(),
            new TemperatureBuilder().WithDate(startDate.AddMinutes(60).ToUniversalTime()).Build(),
            new TemperatureBuilder().WithDate(endDate.AddHours(-30).ToUniversalTime()).Build()
        };

        foreach (var temperature in mockData)
        {
            await _temperatureRepository.Create(temperature, SENSOR_TITLE);
        }

        // act
        var result = await _temperatureRepository.GetByDateRange(startDate, endDate, SENSOR_TITLE);

        // assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(h => h.Date >= startDate && h.Date <= endDate).Should().BeTrue();
    }

    [Fact]
    public async Task GetLatestTemperature_Should_Return_Latest_Temperature()
    {
        // arrange
        var startDate = DateTime.UtcNow.AddMinutes(-30);
        var endDate = DateTime.UtcNow;

        var mockData = new List<Temperature>
        {
            new TemperatureBuilder().WithDate(startDate).Build(),
            new TemperatureBuilder().WithDate(endDate).Build()
        };

        foreach (var temperature in mockData)
        {
            await _temperatureRepository.Create(temperature, SENSOR_TITLE);
        }

        // act
        var result = await _temperatureRepository.GetLatestTemperature(SENSOR_TITLE);

        // assert
        result.Should().NotBeNull();
        result.Date
            .Should()
            .BeCloseTo(mockData.ElementAt(1).Date, precision: TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetLatestTemperature_Should_Throw_NotFoundException_When_No_Temperature_Exists()
    {
        // act
        var act = async () => await _temperatureRepository.GetLatestTemperature(SENSOR_TITLE);

        // assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    public void Dispose()
    {
        _mongoClient.DropDatabase(_databaseName);
    }
}
