using Domain.Domain.Entities;
using MongoDB.Driver;

namespace Infrastructure.Infrastructure.Mongo;

public class MongoSeeder
{
    private readonly IMongoDatabase _database;

    public MongoSeeder(IMongoDatabase database)
    {
        _database = database;
    }

    public void SeedData()
    {
        var collection = _database.GetCollection<Sensor>("Sensor");

        var existingSensors = collection.Find(Builders<Sensor>.Filter.Empty).Any();

        if (!existingSensors)
        {
            var sensorData = new List<Sensor>
            {
                new Sensor("LivingRoom"),
                new Sensor("Bedroom"),
                new Sensor("WorkSpace")
            };

            collection.InsertMany(sensorData);
        }
    }
}
