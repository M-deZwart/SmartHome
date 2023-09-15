using Application.Application.Exceptions;
using Domain.Domain.Contracts;
using Domain.Domain.Entities;
using Domain.Tests.Builders;
using FluentAssertions;
using Infrastructure.Tests.IntegrationTests.TestInfra;
using MongoDB.Driver;
using SmartHomeAPI.Infrastructure.Repositories;

namespace Infrastructure.Tests.IntegrationTests.Mongo;

[Collection("MongoCollection")]
public class TemperatureRepositoryMongoTests : IDisposable
{
    private readonly IMongoDatabase _database;
    private readonly ITemperatureRepository _temperatureRepository;
    private readonly IMongoCollection<Temperature> _temperatureCollection;
    private readonly MongoClient _mongoClient;
    private readonly string _databaseName;
    private const string SENSOR_TITLE = "LivingRoom";

    public TemperatureRepositoryMongoTests(MongoFixture mongoFixture)
    {
        _mongoClient = new MongoClient(mongoFixture.ConnectionString);
        _databaseName = mongoFixture.Database;
        _database = _mongoClient.GetDatabase(_databaseName);

        _temperatureRepository = new TemperatureRepositoryMongo(_database);
        _temperatureCollection = _database.GetCollection<Temperature>("Temperature");
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
        result.Date.Should().BeCloseTo(temperature.Date.ToUniversalTime(), precision: TimeSpan.FromSeconds(1));
        result.Should().BeEquivalentTo(temperature, options => options.Excluding(x => x.Date));
    }

    [Fact]
    public async Task GetByDateRange_Should_Return_TemperatureList_WithinDateRange()
    {
        // arrange
        var startDate = DateTime.UtcNow.AddHours(-24);
        var endDate = DateTime.UtcNow;

        var temperature1 = new TemperatureBuilder().WithDate(startDate.AddMinutes(30).ToUniversalTime()).Build();
        var temperature2 = new TemperatureBuilder().WithDate(startDate.AddMinutes(60).ToUniversalTime()).Build();
        var temperature3 = new TemperatureBuilder().WithDate(endDate.AddHours(-30).ToUniversalTime()).Build();

        await _temperatureCollection
              .InsertManyAsync(new List<Temperature> { temperature1, temperature2, temperature3 });

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

        var temperature1 = new TemperatureBuilder().WithDate(startDate).Build();
        var temperature2 = new TemperatureBuilder().WithDate(endDate).Build();

        await _temperatureCollection.InsertManyAsync(new List<Temperature>
                { temperature1, temperature2 });

        // act
        var result = await _temperatureRepository.GetLatestTemperature(SENSOR_TITLE);

        // assert
        result.Should().NotBeNull();
        result.Date.Should().BeCloseTo(temperature2.Date, precision: TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetLatestTemperature_Should_Throw_NotFoundException_When_No_Temperature_Exists()
    {
        // act
        Func<Task> act = async () => await _temperatureRepository.GetLatestTemperature(SENSOR_TITLE);

        // assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    public void Dispose()
    {
        _mongoClient.DropDatabase(_databaseName);
    }
}
