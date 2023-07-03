using MongoDB.Bson;
using SmartHomeAPI.Application.Entities;

namespace Infrastructure.Infrastructure.Mappers;
public interface IHumidityMongoMapper
{
    Humidity MapFromBsonDocument(BsonDocument document);
    BsonDocument MapToBsonDocument(Humidity humidity);
}