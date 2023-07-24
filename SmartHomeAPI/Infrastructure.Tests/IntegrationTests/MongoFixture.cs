using Mongo2Go;
using MongoDB.Driver;

namespace Infrastructure.Tests.IntegrationTests;

public class MongoFixture : IDisposable
{
    private readonly MongoDbRunner _runner;
    public IMongoClient MongoClient { get; }
    public IMongoDatabase MongoDatabase { get; }

    public MongoFixture()
    {
        _runner = MongoDbRunner.Start();

        var connectionString = _runner.ConnectionString;
        MongoClient = new MongoClient(connectionString);
        MongoDatabase = MongoClient.GetDatabase("test_database");
    }

    public void WaitForConnection()
    {
        var timeout = TimeSpan.FromSeconds(30);
        var startTime = DateTime.UtcNow;

        while (DateTime.UtcNow - startTime < timeout)
        {
            try
            {
                MongoClient.ListDatabaseNames();
                return;
            }
            catch (Exception)
            {
                Thread.Sleep(1000);
            }
        }

        throw new TimeoutException("Could not make a connection with MongoDB-server.");
    }

    public void Dispose()
    {
        _runner?.Dispose();
    }
}
