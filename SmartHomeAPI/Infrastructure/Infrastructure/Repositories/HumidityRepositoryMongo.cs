using Application.Application.Exceptions;
using Domain.Domain.Contracts;
using Domain.Domain.Entities;
using MongoDB.Driver;

namespace SmartHomeAPI.Infrastructure.Repositories
{
    public class HumidityRepositoryMongo : IHumidityRepository
    {
        private readonly IMongoCollection<Humidity> _humidityCollection;

        public HumidityRepositoryMongo(IMongoDatabase db)
        {
            _humidityCollection = db.GetCollection<Humidity>("Humidity");
        }

        public async Task Create(Humidity humidity)
        {
            await _humidityCollection.InsertOneAsync(humidity);
        }

        public async Task<List<Humidity>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            var filter = Builders<Humidity>.Filter.And(
                Builders<Humidity>.Filter.Gte("Date", startDate),
                Builders<Humidity>.Filter.Lte("Date", endDate)
            );

            var humidityList = await _humidityCollection
                .Find(filter)
                .ToListAsync();

            return humidityList;
        }

        public async Task<Humidity> GetLatestHumidity()
        {
            var latestHumidity = await _humidityCollection.Find(_ => true)
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

    }
}
