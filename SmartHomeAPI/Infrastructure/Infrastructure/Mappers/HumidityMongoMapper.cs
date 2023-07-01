using Microsoft.Identity.Client;
using MongoDB.Bson;
using MongoDB.Driver;
using SmartHomeAPI.Application.Entities;

namespace Infrastructure.Infrastructure.Mappers
{
    public class HumidityMongoMapper
    {
        public BsonDocument MapToBsonDocument(Humidity humidity)
        {
            var document = new BsonDocument
            {
                { "_id", BsonValue.Create(humidity.Id) },
                { "Percentage", humidity.Percentage},
                { "Date", humidity.Date }
            };

            return document;
        }

        public Humidity MapFromBsonDocument(BsonDocument document)
        {
            var humidity = new Humidity
            {
                Id = document["_id"].AsGuid,
                Percentage = document["Percentage"].AsDouble,
                Date = document["Date"].ToUniversalTime().AddHours(2)
            };

            return humidity;
        }

    }
}




