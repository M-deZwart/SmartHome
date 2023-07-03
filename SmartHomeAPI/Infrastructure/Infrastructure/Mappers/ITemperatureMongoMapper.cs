using MongoDB.Bson;
using SmartHomeAPI.Application.Entities;

namespace Infrastructure.Infrastructure.Mappers;
public interface ITemperatureMongoMapper
{
    Temperature MapFromBsonDocument(BsonDocument document);
    BsonDocument MapToBsonDocument(Temperature temperature);
}