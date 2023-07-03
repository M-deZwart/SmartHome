using Application.Application.Interfaces;
using Infrastructure.Infrastructure.Mappers;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using SmartHomeAPI.Application.Entities;

namespace SmartHomeAPI.Infrastructure.Repositories
{
    public class TemperatureRepositoryMongo : ITemperatureRepository
    {
        private readonly TemperatureMongoMapper _temperatureMapper;
        private readonly IMongoCollection<BsonDocument> _temperatureCollection;
        private readonly ILogger<TemperatureRepositoryMongo> _logger;

        public TemperatureRepositoryMongo(
            IMongoDatabase db, 
            TemperatureMongoMapper temperatureMapper,
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

        public async Task<Temperature> GetById(Guid id)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
                var temperatureBsonDocument = await _temperatureCollection.Find(filter).FirstOrDefaultAsync();

                if (temperatureBsonDocument is not null)
                {
                    return _temperatureMapper.MapFromBsonDocument(temperatureBsonDocument);
                }
                else
                {
                    throw new InvalidOperationException("Temperature not found");
                }            
            }
            catch (Exception ex)
            {
                var errorMessage = "Failed to get temperature by ID:";
                _logger.LogError(ex, $"{errorMessage} {ex.Message}");
                throw new InvalidOperationException($"{errorMessage} {ex.Message}", ex);
            }
        }

    }
}
