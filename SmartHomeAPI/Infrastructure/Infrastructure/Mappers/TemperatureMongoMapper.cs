using MongoDB.Bson;
using SmartHomeAPI.ApplicationCore.Entities;

namespace Infrastructure.Infrastructure.Mappers
{
    public class TemperatureMongoMapper 
    {
        public BsonDocument MapToBsonDocument(Temperature temperature)
        {
            var document = new BsonDocument
            {
                { "_id", BsonValue.Create(temperature.Id) },
                { "Celsius", temperature.Celsius},
                { "Date", temperature.Date }
            };

            return document;
        }

        public Temperature MapFromBsonDocument(BsonDocument document)
        {
            var temperature = new Temperature
            {
                Id = document["_id"].AsGuid,
                Celsius = document["Celsius"].AsDouble,
                Date = document["Date"].ToUniversalTime().AddHours(2)
            };

            return temperature;
        }

    }
}
