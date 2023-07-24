using Domain.Domain.Contracts;
using Domain.Domain.Entities;
using Domain.Tests.Builders;
using FluentAssertions;
using Infrastructure.Infrastructure.Mappers;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using SmartHomeAPI.Infrastructure.Repositories;

namespace Infrastructure.Tests.IntegrationTests;

[Collection("MongoCollection")]
public class HumidityRepositoryMongoTests : IDisposable
{
    private readonly MongoFixture _mongoFixture;

    public HumidityRepositoryMongoTests(MongoFixture mongoFixture)
    {
        _mongoFixture = mongoFixture;
        _mongoFixture.WaitForConnection();
    }

    private IHumidityRepository CreateHumidityRepository()
    {
        var humidityMapper = new HumidityMongoMapper();
        var logger = new Mock<ILogger<HumidityRepositoryMongo>>().Object;

        return new HumidityRepositoryMongo(
            _mongoFixture.MongoDatabase,
            humidityMapper,
            logger
        );
    }

    [Fact]
    public async Task Create_Should_Add_Humidity_To_Database()
    {
        // arrange
        var humidity = new HumidityBuilder().Build();
        var humidityRepository = CreateHumidityRepository();

        // act
        await humidityRepository.Create(humidity);

        // assert
        var bsonDocument = new BsonDocument
    {
        { "_id", humidity.Id },
        { "Percentage", humidity.Percentage },
        { "Date", humidity.Date.ToUniversalTime() }
    };

        var filter = Builders<BsonDocument>.Filter.Eq("_id", humidity.Id);
        var result = await _mongoFixture.MongoDatabase.GetCollection<BsonDocument>("Humidity").Find(filter).FirstOrDefaultAsync();

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(bsonDocument);
    }

    [Fact]
    public async Task GetByDateRange_Should_Return_HumidityList_WithinDateRange()
    {
        // arrange
        var humidityRepository = CreateHumidityRepository();
        var startDate = DateTime.UtcNow.AddHours(-24);
        var endDate = DateTime.UtcNow;

        var humidity1 = new HumidityBuilder().WithDate(startDate.AddMinutes(30).ToUniversalTime()).Build();
        var humidity2 = new HumidityBuilder().WithDate(startDate.AddMinutes(60).ToUniversalTime()).Build();
        var humidity3 = new HumidityBuilder().WithDate(endDate.AddHours(-30).ToUniversalTime()).Build();

        await _mongoFixture.MongoDatabase
            .GetCollection<Humidity>("Humidity")
            .InsertManyAsync(new List<Humidity> { humidity1, humidity2, humidity3 });

        // act
        var result = await humidityRepository.GetByDateRange(startDate, endDate);

        // assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(h => h.Date >= startDate && h.Date <= endDate).Should().BeTrue();
    }

    [Fact]
    public async Task GetLatestHumidity_Should_Return_Latest_Humidity()
    {
        // arrange
        var humidityRepository = CreateHumidityRepository();
        var startDate = DateTime.UtcNow.AddMinutes(-30);
        var endDate = DateTime.UtcNow;

        var humidity1 = new HumidityBuilder().WithDate(startDate).Build();
        var humidity2 = new HumidityBuilder().WithDate(endDate).Build();

        var humidityCollection = _mongoFixture.MongoDatabase.GetCollection<BsonDocument>("Humidity");
        await humidityCollection.InsertManyAsync(new List<BsonDocument>
            { humidity1.ToBsonDocument(), humidity2.ToBsonDocument() });

        // act
        var result = await humidityRepository.GetLatestHumidity();

        // assert
        result.Should().NotBeNull();
        result.Date.Should().BeCloseTo(humidity2.Date, precision: TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetLatestHumidity_Should_Throw_InvalidOperationException_If_No_Humidity_Found()
    {
        // arrange
        var humidityRepository = CreateHumidityRepository();

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
        var humidityRepository = CreateHumidityRepository();

        // act & assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => humidityRepository.Create(null));
    }

    public void Dispose()
    {
        _mongoFixture.Dispose();
    }
}
