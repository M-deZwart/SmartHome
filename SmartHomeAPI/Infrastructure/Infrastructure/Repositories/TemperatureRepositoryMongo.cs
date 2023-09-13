using Application.Application.Exceptions;
using Domain.Domain.Contracts;
using Domain.Domain.Entities;
using MongoDB.Driver;

namespace SmartHomeAPI.Infrastructure.Repositories
{
    public class TemperatureRepositoryMongo : ITemperatureRepository
    {
        private readonly IMongoCollection<Temperature> _temperatureCollection;
        private readonly IMongoCollection<Sensor> _sensorCollection;

        public TemperatureRepositoryMongo(IMongoDatabase db)
        {
            _temperatureCollection = db.GetCollection<Temperature>("Temperature");
            _sensorCollection = db.GetCollection<Sensor>("Sensor");
        }

        public async Task Create(Temperature temperature, string sensorTitle)
        {
            var sensor = await FindSensor(sensorTitle);
            temperature.SensorId = sensor.Id;

            await _temperatureCollection.InsertOneAsync(temperature);
        }

        public async Task<List<Temperature>> GetByDateRange(DateTime startDate, DateTime endDate, string sensorTitle)
        {
            var sensor = await FindSensor(sensorTitle);

            var filter = Builders<Temperature>.Filter.And(
                Builders<Temperature>.Filter.Where(t => t.SensorId == sensor.Id),
                Builders<Temperature>.Filter.Gte("Date", startDate),
                Builders<Temperature>.Filter.Lte("Date", endDate)
            );

            var temperatureList = await _temperatureCollection
                .Find(filter)
                .ToListAsync();

            return temperatureList;
        }

        public async Task<Temperature> GetLatestTemperature(string sensorTitle)
        {
            var sensor = await FindSensor(sensorTitle);
            var filter = Builders<Temperature>.Filter.Where(t => t.SensorId == sensor.Id);

            var latestTemperature = await _temperatureCollection.Find(filter)
                .SortByDescending(temperature => temperature.Date)
                .FirstOrDefaultAsync();

            if (latestTemperature is not null)
            {
                return latestTemperature;
            }
            else
            {
                throw new InvalidOperationException($"Temperature was not found");
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
