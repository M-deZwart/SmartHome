﻿using Domain.Domain.Contracts;
using Domain.Tests.Builders;
using FluentAssertions;
using Infrastructure.Infrastructure.Mappers;
using Infrastructure.Tests.IntegrationTests.TestInfra;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using SmartHomeAPI.Infrastructure.Repositories;

namespace Infrastructure.Tests.IntegrationTests.Mongo;

[Collection("MongoCollection")]
public class TemperatureRepositoryMongoTests : IDisposable
{
    private readonly IMongoDatabase _database;
    private readonly ITemperatureRepository _temperatureRepository;
    private readonly IMongoCollection<BsonDocument> _temperatureCollection;
    private readonly MongoClient _mongoClient;
    private readonly string _databaseName;

    public TemperatureRepositoryMongoTests(MongoFixture mongoFixture)
    {
        _mongoClient = new MongoClient(mongoFixture.ConnectionString);
        _databaseName = mongoFixture.Database;
        _database = _mongoClient.GetDatabase(_databaseName);

        var temperatureMapper = new TemperatureMongoMapper();
        var logger = new Mock<ILogger<TemperatureRepositoryMongo>>().Object;

        _temperatureRepository = new TemperatureRepositoryMongo(
                _database,
                temperatureMapper,
                logger
            );

        _temperatureCollection = _database.GetCollection<BsonDocument>("Temperature");
    }

    [Fact]
    public async Task Create_Should_Add_Temperature_To_Database()
    {
        // arrange
        var temperature = new TemperatureBuilder().Build();

        // act
        await _temperatureRepository.Create(temperature);

        // assert
        var bsonDocument = new BsonDocument
        {
            { "_id", temperature.Id },
            { "Celsius", temperature.Celsius },
            { "Date", temperature.Date.ToUniversalTime() }
        };

        var filter = Builders<BsonDocument>.Filter.Eq("_id", temperature.Id);
        var result = await _temperatureCollection.Find(filter).FirstOrDefaultAsync();

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(bsonDocument);
    }

    [Fact]
    public async Task GetByDateRange_Should_Return_TemperatureList_WithinDateRange()
    {
        // arrange
        var startDate = DateTime.UtcNow.AddHours(-24);
        var endDate = DateTime.UtcNow;

        var temperature1 = new TemperatureBuilder().WithDate(startDate.AddMinutes(30).ToUniversalTime()).Build().ToBsonDocument();
        var temperature2 = new TemperatureBuilder().WithDate(startDate.AddMinutes(60).ToUniversalTime()).Build().ToBsonDocument();
        var temperature3 = new TemperatureBuilder().WithDate(endDate.AddHours(-30).ToUniversalTime()).Build().ToBsonDocument();

        await _temperatureCollection
              .InsertManyAsync(new List<BsonDocument> { temperature1, temperature2, temperature3 });

        // act
        var result = await _temperatureRepository.GetByDateRange(startDate, endDate);

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

        await _temperatureCollection.InsertManyAsync(new List<BsonDocument>
                { temperature1.ToBsonDocument(), temperature2.ToBsonDocument() });

        // act
        var result = await _temperatureRepository.GetLatestTemperature();

        // assert
        result.Should().NotBeNull();
        result.Date.Should().BeCloseTo(temperature2.Date, precision: TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetLatestTemperature_Should_Throw_InvalidOperationException_If_Humidity_NotFound()
    {
        // act
        var act = _temperatureRepository.GetLatestTemperature;

        // assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Failed to get current temperature: Temperature was not found");
    }

    [Fact]
    public async Task Create_NullValue_ThrowsInvalidOperationException()
    {
        // act & assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _temperatureRepository.Create(null));
    }

    public async void Dispose()
    {
        _mongoClient.DropDatabase(_databaseName);
    }
}
