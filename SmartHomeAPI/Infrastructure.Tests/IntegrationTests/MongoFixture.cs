using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Tests.IntegrationTests;

public class MongoFixture : IDisposable
{
    private readonly MongoDbRunner _runner;
    public IMongoClient MongoClient { get; }
    public IMongoDatabase MongoDatabase { get; }
    public IMongoCollection<BsonDocument> HumidityCollection { get; }
    public IMongoCollection<BsonDocument> TemperatureCollection { get; }

    public MongoFixture()
    {
        _runner = MongoDbRunner.Start();

        var connectionString = _runner.ConnectionString;
        MongoClient = new MongoClient(connectionString);
        MongoDatabase = MongoClient.GetDatabase("test_database");
        HumidityCollection = MongoDatabase.GetCollection<BsonDocument>("Humidity");
        TemperatureCollection = MongoDatabase.GetCollection<BsonDocument>("Temperature");
    }

    public void Dispose()
    {
        _runner?.Dispose();
    }
}
