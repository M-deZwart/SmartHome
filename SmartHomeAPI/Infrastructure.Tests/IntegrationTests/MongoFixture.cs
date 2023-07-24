using Domain.Domain.Entities;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Tests.IntegrationTests;

public class MongoFixture : IDisposable
{
    private static MongoDbRunner _runner;
    public IMongoClient MongoClient { get; }
    public IMongoDatabase MongoDatabase { get; }

    public MongoFixture()
    {
        _runner = MongoDbRunner.Start();

        var connectionString = _runner.ConnectionString;
        MongoClient = new MongoClient(connectionString);
        MongoDatabase = MongoClient.GetDatabase("test_database");
    }

    public void Dispose ()
    {
        _runner?.Dispose();
    }
}
