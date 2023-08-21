using Domain.Domain.Entities;
using MongoDB.Bson.Serialization;

namespace Infrastructure.Infrastructure;
public static class MongoDbConfig
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<Humidity>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
            cm.SetIdMember(cm.GetMemberMap(c => c.Id));
        });

        BsonClassMap.RegisterClassMap<Temperature>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
            cm.SetIdMember(cm.GetMemberMap(c => c.Id));
        });
    }
}