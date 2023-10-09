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
public class HumidityRepositoryMongoTests : IDisposable
{
    private readonly IMongoDatabase _database;
    private readonly MongoClient _mongoClient;
    private readonly string _databaseName;

    private readonly IMongoCollection<Humidity> _humidityCollection;
    private readonly IMongoCollection<Sensor> _sensorCollection;

    private readonly IHumidityRepository _humidityRepository;
    private readonly Sensor _sensor;

    private const string SENSOR_TITLE = "LivingRoom";

    public HumidityRepositoryMongoTests(MongoFixture mongoFixture)
    {
        _mongoClient = new MongoClient(mongoFixture.ConnectionString);
        _databaseName = mongoFixture.Database;
        _database = _mongoClient.GetDatabase(_databaseName);

        _humidityRepository = new HumidityRepositoryMongo(_database);
        _humidityCollection = _database.GetCollection<Humidity>("Humidity");

        _sensor = new Sensor(SENSOR_TITLE);
        _sensorCollection = _database.GetCollection<Sensor>("Sensor");
        _sensorCollection.InsertOne(_sensor);
    }

    [Fact]
    public async Task Create_Should_Add_Humidity_To_Database()
    {
        // arrange
        var humidity = new HumidityBuilder().Build();

        // act
        await _humidityRepository.Create(humidity, SENSOR_TITLE);

        // assert
        var filter = Builders<Humidity>.Filter.Eq("Percentage", humidity.Percentage);
        var result = await _humidityCollection.Find(filter).FirstOrDefaultAsync();

        result.Should().NotBeNull();
        result.Date
            .Should()
            .BeCloseTo(humidity.Date.ToUniversalTime(), precision: TimeSpan.FromSeconds(1));
        result.Should().BeEquivalentTo(humidity, options => options.Excluding(x => x.Date));
    }

    [Fact]
    public async Task GetByDateRange_Should_Return_HumidityList_WithinDateRange()
    {
        // arrange
        var startDate = DateTime.UtcNow.AddHours(-24);
        var endDate = DateTime.UtcNow;

        var mockData = new List<Humidity>
        {
            new HumidityBuilder().WithDate(startDate.AddMinutes(30).ToUniversalTime()).Build(),
            new HumidityBuilder().WithDate(startDate.AddMinutes(60).ToUniversalTime()).Build(),
            new HumidityBuilder().WithDate(endDate.AddHours(-30).ToUniversalTime()).Build(),
        };

        foreach (var humidity in mockData)
        {
            await _humidityRepository.Create(humidity, SENSOR_TITLE);
        }

        // act
        var result = await _humidityRepository.GetByDateRange(startDate, endDate, SENSOR_TITLE);

        // assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(h => h.Date >= startDate && h.Date <= endDate).Should().BeTrue();
    }

    [Fact]
    public async Task GetLatestHumidity_Should_Return_Latest_Humidity()
    {
        // arrange
        var date1 = DateTime.UtcNow.AddMinutes(-30);
        var date2 = DateTime.UtcNow;

        var mockData = new List<Humidity>
        {
            new HumidityBuilder().WithDate(date1).Build(),
            new HumidityBuilder().WithDate(date2).Build()
        };

        foreach (var humidity in mockData)
        {
            await _humidityRepository.Create(humidity, SENSOR_TITLE);
        }

        // act
        var result = await _humidityRepository.GetLatestHumidity(SENSOR_TITLE);

        // assert
        result.Should().NotBeNull();
        result.Date
            .Should()
            .BeCloseTo(mockData.ElementAt(1).Date, precision: TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetLatestHumidity_Should_Throw_NotFoundException_When_No_Humidity_Exists()
    {
        // act
        var act = async () => await _humidityRepository.GetLatestHumidity(SENSOR_TITLE);

        // assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    public void Dispose()
    {
        _mongoClient.DropDatabase(_databaseName);
    }
}
