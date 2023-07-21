using Domain.Tests.Builders;
using FluentAssertions;
using Infrastructure.Infrastructure.Mappers;
using MongoDB.Bson;

namespace Infrastructure.Tests;

public class TemperatureMongoMapperTests
{
    private readonly ITemperatureMongoMapper _mapper;

    public TemperatureMongoMapperTests()
    {
        _mapper = new TemperatureMongoMapper();
    }

    [Fact]
    public void TestMapTemperatureToBsonDocument()
    {
        // arrange
        var temperature = new TemperatureBuilder().Build();
        BsonDocument bsonTemperature = temperature.ToBsonDocument();

        // act
        var mappedTemperature = _mapper.MapToBsonDocument(temperature);

        // assert
        mappedTemperature.Should().BeEquivalentTo(bsonTemperature);
    }

    [Fact]
    public void TestMapTemperatureFromBsonDocument()
    {
        // arrange
        BsonDocument bsonTemperature = new BsonDocument() {
            { "_id", BsonValue.Create(Guid.NewGuid()) },
            { "Celsius", 19.5 },
            { "Date", BsonValue.Create(DateTime.UtcNow) }
        };

        var temperature = new TemperatureBuilder()
            .WithId(bsonTemperature["_id"].AsGuid)
            .WithCelsius(bsonTemperature["Celsius"].AsDouble)
            .WithDate(bsonTemperature["Date"].ToUniversalTime())
            .Build();

        // act
        var mappedTemperature = _mapper.MapFromBsonDocument(bsonTemperature);

        // assert
        mappedTemperature.Should().BeEquivalentTo(mappedTemperature);
    }
}
