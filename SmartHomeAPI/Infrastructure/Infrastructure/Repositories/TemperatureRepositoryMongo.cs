using Domain.Domain.Contracts;
using Domain.Domain.Entities;
using Infrastructure.Infrastructure.Mappers;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace SmartHomeAPI.Infrastructure.Repositories
{
    public class TemperatureRepositoryMongo : ITemperatureRepository
    {
        private readonly IMongoCollection<Temperature> _temperatureCollection;

        public TemperatureRepositoryMongo(IMongoDatabase db)
        {
            _temperatureCollection = db.GetCollection<Temperature>("Temperature");
        }

        public async Task Create(Temperature temperature)
        {
            temperature.Date = temperature.Date.ToUniversalTime().AddHours(2);
            await _temperatureCollection.InsertOneAsync(temperature);
        }

        public async Task<List<Temperature>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            var filter = Builders<Temperature>.Filter.And(
                Builders<Temperature>.Filter.Gte("Date", startDate),
                Builders<Temperature>.Filter.Lte("Date", endDate)
            );

            var temperatureList = await _temperatureCollection
                .Find(filter)
                .ToListAsync();

            return temperatureList;
        }

        public async Task<Temperature> GetLatestTemperature()
        {
            var latestTemperature = await _temperatureCollection.Find(_ => true)
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

    }
}
