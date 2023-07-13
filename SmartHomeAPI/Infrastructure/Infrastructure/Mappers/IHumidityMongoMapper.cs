using Domain.Domain.Entities;
using MongoDB.Bson;

namespace Infrastructure.Infrastructure.Mappers;
public interface IHumidityMongoMapper
{
    Humidity MapFromBsonDocument(BsonDocument document);
    BsonDocument MapToBsonDocument(Humidity humidity);
}