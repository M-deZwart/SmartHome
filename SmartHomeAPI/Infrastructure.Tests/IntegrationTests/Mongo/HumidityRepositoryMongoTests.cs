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
public class HumidityRepositoryMongoTests : IDisposable
{
    private readonly IMongoDatabase _database;
    private readonly IHumidityRepository _humidityRepository;
    private readonly IMongoCollection<Humidity> _humidityCollection;
    private readonly MongoClient _mongoClient;
    private readonly string _databaseName;
    private const string SENSOR_TITLE = "LivingRoom";

    public HumidityRepositoryMongoTests(MongoFixture mongoFixture)
    {
        _mongoClient = new MongoClient(mongoFixture.ConnectionString);
        _databaseName = mongoFixture.Database;
        _database = _mongoClient.GetDatabase(_databaseName);

        _humidityRepository = new HumidityRepositoryMongo(_database);
        _humidityCollection = _database.GetCollection<Humidity>("Humidity");
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
        result.Date.Should().BeCloseTo(humidity.Date.ToUniversalTime(), precision: TimeSpan.FromSeconds(1));
        result.Should().BeEquivalentTo(humidity, options => options.Excluding(x => x.Date));
    }

    [Fact]
    public async Task GetByDateRange_Should_Return_HumidityList_WithinDateRange()
    {
        // arrange
        var startDate = DateTime.UtcNow.AddHours(-24);
        var endDate = DateTime.UtcNow;

        var humidity1 = new HumidityBuilder().WithDate(startDate.AddMinutes(30).ToUniversalTime()).Build();
        var humidity2 = new HumidityBuilder().WithDate(startDate.AddMinutes(60).ToUniversalTime()).Build();
        var humidity3 = new HumidityBuilder().WithDate(endDate.AddHours(-30).ToUniversalTime()).Build();

        await _humidityCollection
              .InsertManyAsync(new List<Humidity> { humidity1, humidity2, humidity3 });

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

        var humidity1 = new HumidityBuilder().WithDate(date1).Build();
        var humidity2 = new HumidityBuilder().WithDate(date2).Build();

        await _humidityCollection.InsertManyAsync(new List<Humidity>
            { humidity1, humidity2 });

        // act
        var result = await _humidityRepository.GetLatestHumidity(SENSOR_TITLE);

        // assert
        result.Should().NotBeNull();
        result.Date.Should().BeCloseTo(humidity2.Date, precision: TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetLatestHumidity_Should_Throw_NotFoundException_When_No_Humidity_Exists()
    {
        // act
        Func<Task> act = async () => await _humidityRepository.GetLatestHumidity(SENSOR_TITLE);

        // assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    public void Dispose()
    {
        _mongoClient.DropDatabase(_databaseName);
    }
}
