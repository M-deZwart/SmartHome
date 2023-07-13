using Domain.Domain.Entities;
using MongoDB.Bson;

namespace Infrastructure.Infrastructure.Mappers;
public interface ITemperatureMongoMapper
{
    Temperature MapFromBsonDocument(BsonDocument document);
    BsonDocument MapToBsonDocument(Temperature temperature);
}