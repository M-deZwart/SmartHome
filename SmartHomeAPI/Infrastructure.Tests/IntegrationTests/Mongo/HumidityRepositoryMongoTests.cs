using Domain.Domain.Contracts;
using Domain.Tests.Builders;
using FluentAssertions;
using Infrastructure.Infrastructure.Mappers;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using SmartHomeAPI.Infrastructure.Repositories;

namespace Infrastructure.Tests.IntegrationTests.Mongo;

[Collection("MongoCollection")]
public class HumidityRepositoryMongoTests 
{
    private readonly IMongoDatabase _database;
    private readonly IHumidityRepository _humidityRepository;
    private readonly IMongoCollection<BsonDocument> _humidityCollection;

    public HumidityRepositoryMongoTests()
    {
        var mongoClient = new MongoClient("mongodb://localhost:27017");
        _database = mongoClient.GetDatabase("smarthome");

        var humidityMapper = new HumidityMongoMapper();
        var logger = new Mock<ILogger<HumidityRepositoryMongo>>().Object;

        _humidityRepository = new HumidityRepositoryMongo(
                _database,
                humidityMapper,
                logger
            );

        _humidityCollection = _database.GetCollection<BsonDocument>("Humidity");
    }

    [Fact]
    public async Task Create_Should_Add_Humidity_To_Database()
    {
        // arrange
        var humidity = new HumidityBuilder().Build();

        // act
        await _humidityRepository.Create(humidity);

        // assert
        var bsonDocument = new BsonDocument
        {
            { "_id", humidity.Id },
            { "Percentage", humidity.Percentage },
            { "Date", humidity.Date.ToUniversalTime() }
        };

        var filter = Builders<BsonDocument>.Filter.Eq("_id", humidity.Id);
        var result = await _humidityCollection.Find(filter).FirstOrDefaultAsync();

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(bsonDocument);
    }

    [Fact]
    public async Task GetByDateRange_Should_Return_HumidityList_WithinDateRange()
    {
        // arrange
        var startDate = DateTime.UtcNow.AddHours(-24);
        var endDate = DateTime.UtcNow;

        var humidity1 = new HumidityBuilder().WithDate(startDate.AddMinutes(30).ToUniversalTime()).Build().ToBsonDocument();
        var humidity2 = new HumidityBuilder().WithDate(startDate.AddMinutes(60).ToUniversalTime()).Build().ToBsonDocument();
        var humidity3 = new HumidityBuilder().WithDate(endDate.AddHours(-30).ToUniversalTime()).Build().ToBsonDocument();

        await _humidityCollection
              .InsertManyAsync(new List<BsonDocument> { humidity1, humidity2, humidity3 });

        // act
        var result = await _humidityRepository.GetByDateRange(startDate, endDate);

        // assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(h => h.Date >= startDate && h.Date <= endDate).Should().BeTrue();
    }

    [Fact]
    public async Task GetLatestHumidity_Should_Return_Latest_Humidity()
    {
        // arrange
        var startDate = DateTime.UtcNow.AddMinutes(-30);
        var endDate = DateTime.UtcNow;

        var humidity1 = new HumidityBuilder().WithDate(startDate).Build();
        var humidity2 = new HumidityBuilder().WithDate(endDate).Build();

        await _humidityCollection.InsertManyAsync(new List<BsonDocument>
            { humidity1.ToBsonDocument(), humidity2.ToBsonDocument() });

        // act
        var result = await _humidityRepository.GetLatestHumidity();

        // assert
        result.Should().NotBeNull();
        result.Date.Should().BeCloseTo(humidity2.Date, precision: TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetLatestHumidity_Should_Throw_InvalidOperationException_If_No_Humidity_Found()
    {
        // act
        var act = _humidityRepository.GetLatestHumidity;

        // assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Failed to get current humidity: Humidity was not found");
    }

    [Fact]
    public async Task Create_NullValue_ThrowsInvalidOperationException()
    {
        // act & assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _humidityRepository.Create(null));
    }

}
