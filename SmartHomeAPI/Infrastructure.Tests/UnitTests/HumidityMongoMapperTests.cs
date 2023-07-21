using Domain.Tests.Builders;
using FluentAssertions;
using Infrastructure.Infrastructure.Mappers;
using MongoDB.Bson;

namespace Infrastructure.Tests.UnitTests;

public class HumidityMongoMapperTests
{
    private readonly IHumidityMongoMapper _mapper;

    public HumidityMongoMapperTests()
    {
        _mapper = new HumidityMongoMapper();
    }

    [Fact]
    public void TestMapHumidityToBsonDocument()
    {
        //arrange
        var humidity = new HumidityBuilder().Build();
        var bsonHumidity = humidity.ToBsonDocument();

        // act
        var mappedHumidity = _mapper.MapToBsonDocument(humidity);

        // assert
        mappedHumidity.Should().BeEquivalentTo(bsonHumidity);
    }

    [Fact]
    public void TestMapHumidityFromBsonDocument()
    {
        // arrange
        var bsonHumidity = new BsonDocument
        {
            { "_id", BsonValue.Create(Guid.NewGuid()) },
            { "Percentage", 80.5 },
            { "Date", BsonValue.Create(DateTime.UtcNow) }
        };

        var humidity = new HumidityBuilder()
            .WithId(bsonHumidity["_id"].AsGuid)
            .WithPercentage(bsonHumidity["Percentage"].AsDouble)
            .WithDate(bsonHumidity["Date"].ToUniversalTime())
            .Build();

        // act
        var mappedHumidity = _mapper.MapFromBsonDocument(bsonHumidity);

        // assert
        mappedHumidity.Should().BeEquivalentTo(humidity);
    }
}
