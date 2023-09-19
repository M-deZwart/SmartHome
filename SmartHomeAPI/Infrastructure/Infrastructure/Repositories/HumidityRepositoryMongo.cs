using Application.Application.Exceptions;
using Domain.Domain.Contracts;
using Domain.Domain.Entities;
using MongoDB.Driver;

namespace SmartHomeAPI.Infrastructure.Repositories
{
    public class HumidityRepositoryMongo : IHumidityRepository
    {
        private readonly IMongoCollection<Humidity> _humidityCollection;
        private readonly IMongoCollection<Sensor> _sensorCollection;

        public HumidityRepositoryMongo(IMongoDatabase db)
        {
            _humidityCollection = db.GetCollection<Humidity>("Humidity");
            _sensorCollection = db.GetCollection<Sensor>("Sensor");
        }

        public async Task Create(Humidity humidity, string sensorTitle)
        {
            var sensor = await FindSensor(sensorTitle);
            humidity.SensorId = sensor.Id;

            await _humidityCollection.InsertOneAsync(humidity);
        }

        public async Task<List<Humidity>> GetByDateRange(DateTime startDate, DateTime endDate, string sensorTitle)
        {
            var sensor = await FindSensor(sensorTitle);

            var filter = Builders<Humidity>.Filter.And(
                Builders<Humidity>.Filter.Where(h => h.SensorId == sensor.Id),
                Builders<Humidity>.Filter.Gte("Date", startDate),
                Builders<Humidity>.Filter.Lte("Date", endDate)
            );

            var humidityList = await _humidityCollection
                .Find(filter)
                .ToListAsync();

            return humidityList;
        }

        public async Task<Humidity> GetLatestHumidity(string sensorTitle)
        {
            var sensor = await FindSensor(sensorTitle);
            var filter = Builders<Humidity>.Filter.Where(h => h.SensorId == sensor.Id);

            var latestHumidity = await _humidityCollection.Find(filter)
                .SortByDescending(humidity => humidity.Date)
                .FirstOrDefaultAsync();

            if (latestHumidity is not null)
            {
                return latestHumidity;
            }
            else
            {
                throw new NotFoundException($"Humidity was not found");
            }
        }

        private async Task<Sensor> FindSensor(string sensorTitle)
        {
            var filter = Builders<Sensor>.Filter.Where(s => s.Title == sensorTitle);
            var sensor = await _sensorCollection.Find(filter).FirstOrDefaultAsync();

            if (sensor is not null)
            {
                return sensor;
            }
            else
            {
                throw new NotFoundException("Sensor is not found");
            }
        }

    }
}
