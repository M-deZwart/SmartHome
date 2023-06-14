using MongoDB.Driver;
using SmartHomeAPI.ApplicationCore.Entities;
using SmartHomeAPI.ApplicationCore.Interfaces;

namespace SmartHomeAPI.Infrastructure
{
    public class HumidityRepository : IHumidityRepository
    {
        private IMongoCollection<Humidity> _humidityCollection;

        public HumidityRepository(IMongoDatabase db)
        {
            _humidityCollection = db.GetCollection<Humidity>("Humidity");
        }

        public void Create(Humidity humidity)
        {
            humidity.Date = humidity.Date.ToUniversalTime().AddHours(2);
            _humidityCollection.InsertOne(humidity);
        }

        public List<Humidity> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.ToUniversalTime();
            endDate = endDate.ToUniversalTime();

            var filter = Builders<Humidity>.Filter.And(
                    Builders<Humidity>.Filter.Gte(h => h.Date, startDate),
                    Builders<Humidity>.Filter.Lte(h => h.Date, endDate)
                );

            var humidityList = _humidityCollection
                .Find(filter)
                .ToList();

            return humidityList;
        }
    }
}
