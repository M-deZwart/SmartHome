using MongoDB.Driver;
using SmartHomeAPI.ApplicationCore.Entities;
using SmartHomeAPI.ApplicationCore.Interfaces;

namespace SmartHomeAPI.Infrastructure
{
    public class TemperatureRepository : ITemperatureRepository
    {
        private IMongoCollection<Temperature> _temperatureCollection;

        public TemperatureRepository(IMongoDatabase db)
        {
            _temperatureCollection = db.GetCollection<Temperature>("Temperature");
        }

        public void Create(Temperature temperature)
        {
            temperature.Date = temperature.Date.ToUniversalTime().AddHours(2);
            _temperatureCollection.InsertOne(temperature);
        }

        public List<Temperature> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.ToUniversalTime();
            endDate = endDate.ToUniversalTime();

            var filter = Builders<Temperature>.Filter.And(
                    Builders<Temperature>.Filter.Gte(t => t.Date, startDate),
                    Builders<Temperature>.Filter.Lte(t => t.Date, endDate)
                );

            var temperatureList = _temperatureCollection
                .Find(filter)
                .ToList();

            return temperatureList;
        }
    }
}
