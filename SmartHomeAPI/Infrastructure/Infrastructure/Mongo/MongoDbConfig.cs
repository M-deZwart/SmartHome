using Domain.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace Infrastructure.Infrastructure.Mongo;

public static class MongoDbConfig
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<Humidity>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);

            cm.MapProperty(h => h.Id)
                .SetIdGenerator(CombGuidGenerator.Instance)
                .SetSerializer(new GuidSerializer(BsonType.String));

            cm.MapProperty(h => h.SensorId).SetSerializer(new GuidSerializer(BsonType.String));

            cm.MapMember(c => c.Sensor).SetIgnoreIfNull(true);
        });

        BsonClassMap.RegisterClassMap<Temperature>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);

            cm.MapProperty(t => t.Id)
                .SetIdGenerator(CombGuidGenerator.Instance)
                .SetSerializer(new GuidSerializer(BsonType.String));

            cm.MapProperty(t => t.SensorId).SetSerializer(new GuidSerializer(BsonType.String));

            cm.MapMember(c => c.Sensor).SetIgnoreIfNull(true);
        });

        BsonClassMap.RegisterClassMap<Sensor>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);

            cm.MapProperty(s => s.Id)
                .SetIdGenerator(CombGuidGenerator.Instance)
                .SetSerializer(new GuidSerializer(BsonType.String));

            cm.MapMember(c => c.Humidities)
                .SetShouldSerializeMethod(obj => ((Sensor)obj).Humidities.Any());
            cm.MapMember(c => c.Temperatures)
                .SetShouldSerializeMethod(obj => ((Sensor)obj).Temperatures.Any());
        });
    }
}
