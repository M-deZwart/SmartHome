using Application.Application.Interfaces;
using Domain.Domain.Entities;
using Infrastructure.Infrastructure.Mappers;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace SmartHomeAPI.Infrastructure.Repositories
{
    public class TemperatureRepositoryMongo : ITemperatureRepository
    {
        private readonly ITemperatureMongoMapper _temperatureMapper;
        private readonly IMongoCollection<BsonDocument> _temperatureCollection;
        private readonly ILogger<TemperatureRepositoryMongo> _logger;

        public TemperatureRepositoryMongo(
            IMongoDatabase db, 
            ITemperatureMongoMapper temperatureMapper,
            ILogger<TemperatureRepositoryMongo> logger)
        {
            _temperatureCollection = db.GetCollection<BsonDocument>("Temperature");
            _temperatureMapper = temperatureMapper;
            _logger = logger;
        }

        public async Task Create(Temperature temperature)
        {
            try
            {
                temperature.Date = temperature.Date.ToUniversalTime().AddHours(2);
                var temperatureBsonDocument = _temperatureMapper.MapToBsonDocument(temperature);
                await _temperatureCollection.InsertOneAsync(temperatureBsonDocument);
            }
            catch (Exception ex)
            {
                var errorMessage = "Failed to create temperature:";
                _logger.LogError(ex, $"{errorMessage} {ex.Message}");
                throw new InvalidOperationException($"{errorMessage} {ex.Message}", ex);
            }   
        }

        public async Task<List<Temperature>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Gte("Date", startDate),
                    Builders<BsonDocument>.Filter.Lte("Date", endDate)
                );

                var temperatureBsonDocumentList = await _temperatureCollection
                    .Find(filter)
                    .ToListAsync();

                List<Temperature> temperatureList = new List<Temperature>();

                temperatureBsonDocumentList.ForEach(temperatureBsonDocument =>
                {
                    var temperature = _temperatureMapper.MapFromBsonDocument(temperatureBsonDocument);
                    temperatureList.Add(temperature);
                });

                return temperatureList;
            }
            catch(Exception ex)
            {
                var errorMessage = "Failed to get temperature by date range:";
                _logger.LogError(ex, $"{errorMessage} {ex.Message}");
                throw new InvalidOperationException($"{errorMessage} {ex.Message}", ex);
            }        
        }

        public async Task<Temperature> GetLatestTemperature()
        {
            try
            {
                var latestTemperatureDocument = await _temperatureCollection.Find(_ => true)
                    .SortByDescending(document => document["Date"])
                    .FirstOrDefaultAsync();

                if (latestTemperatureDocument is not null)
                {
                    return _temperatureMapper.MapFromBsonDocument(latestTemperatureDocument);
                }
                else
                {
                    throw new InvalidOperationException($"Temperature was not found");
                }            
            }
            catch (Exception ex)
            {
                var errorMessage = "Failed to get current temperature:";
                _logger.LogError(ex, $"{errorMessage} {ex.Message}");
                throw new InvalidOperationException($"{errorMessage} {ex.Message}", ex);
            }
        }

    }
}
