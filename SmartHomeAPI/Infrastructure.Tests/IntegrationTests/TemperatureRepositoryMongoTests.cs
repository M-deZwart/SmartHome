﻿using Domain.Domain.Contracts;
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

public class TemperatureRepositoryMongoTests : IClassFixture<MongoFixture>, IDisposable
{
    private readonly MongoFixture _mongoFixture;
    private readonly ITemperatureRepository _temperatureRepository;

    public TemperatureRepositoryMongoTests(MongoFixture mongoFixture)
    {
        _mongoFixture = mongoFixture;

        var temperatureMapper = new TemperatureMongoMapper();
        var logger = new Mock<ILogger<TemperatureRepositoryMongo>>().Object;

        _temperatureRepository = new TemperatureRepositoryMongo(
            _mongoFixture.MongoDatabase,
            temperatureMapper,
            logger
        );
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
        var result = await _mongoFixture.MongoDatabase.GetCollection<BsonDocument>("Temperature").Find(filter).FirstOrDefaultAsync();

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(bsonDocument);
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

        await _mongoFixture.MongoDatabase
            .GetCollection<Temperature>("Temperature")
            .InsertManyAsync(new List<Temperature> { temperature1, temperature2, temperature3 });

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

        var temperatureCollection = _mongoFixture.MongoDatabase.GetCollection<BsonDocument>("Temperature");
        await temperatureCollection.InsertManyAsync(new List<BsonDocument>
            { temperature1.ToBsonDocument(), temperature2.ToBsonDocument() });

        // act
        var result = await _temperatureRepository.GetLatestTemperature();

        // assert
        result.Should().NotBeNull();
        result.Date.Should().BeCloseTo(temperature2.Date, precision: TimeSpan.FromSeconds(1));
    }

    public void Dispose()
    {
        _mongoFixture.Dispose();
    }
}
