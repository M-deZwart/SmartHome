using Application.Application.DTOs;
using Application.Application.Interfaces;
using Infrastructure.Infrastructure.Mappers;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace SmartHomeAPI.Infrastructure.Repositories
{
    public class HumidityRepositoryMongo : IHumidityRepository
    {
        private readonly IHumidityMongoMapper _humidityMapper;
        private readonly IMongoCollection<BsonDocument> _humidityCollection;
        private readonly ILogger<HumidityRepositoryMongo> _logger;

        public HumidityRepositoryMongo(
            IMongoDatabase db, 
            IHumidityMongoMapper humidityMapper,
            ILogger<HumidityRepositoryMongo> logger)
        {
            _humidityCollection = db.GetCollection<BsonDocument>("Humidity");
            _humidityMapper = humidityMapper;
            _logger = logger;
        }

        public async Task Create(Humidity humidity)
        {  
            try
            {
                humidity.Date = humidity.Date.ToUniversalTime().AddHours(2);
                var humidityBsonDocument = _humidityMapper.MapToBsonDocument(humidity);
                await _humidityCollection.InsertOneAsync(humidityBsonDocument);
            }
            catch(Exception ex)
            {
                var errorMessage = "Failed to create humidity:";
                _logger.LogError(ex, $"{errorMessage} {ex.Message}");
                throw new InvalidOperationException($"{errorMessage} {ex.Message}", ex);
            }
        }

        public async Task<List<Humidity>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Gte("Date", startDate),
                    Builders<BsonDocument>.Filter.Lte("Date", endDate)
                );

                var humidityBsonDocumentList = await _humidityCollection
                    .Find(filter)
                    .ToListAsync();

                List<Humidity> humidityList = new List<Humidity>();

                humidityBsonDocumentList.ForEach(humidityBsonDocument =>
                {
                    var humidity = _humidityMapper.MapFromBsonDocument(humidityBsonDocument);
                    humidityList.Add(humidity);
                });

                return humidityList;
            }
            catch (Exception ex)
            {
                var errorMessage = "Failed to get humidity by date range:";
                _logger.LogError(ex, $"{errorMessage} {ex.Message}");
                throw new InvalidOperationException($"{errorMessage} {ex.Message}", ex);
            }
        }

        public async Task<Humidity> GetById(Guid id)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
                var humidityBsonDocument = await _humidityCollection.Find(filter).FirstOrDefaultAsync();

                if (humidityBsonDocument is not null)
                {
                    return _humidityMapper.MapFromBsonDocument(humidityBsonDocument);
                }
                else
                {
                    throw new InvalidOperationException($"Humidity with ID: {id} could not be found");
                }            
            } 
            catch (Exception ex)
            {
                var errorMessage = "Failed to get humidity by ID:";
                _logger.LogError(ex, $"{errorMessage} {ex.Message}");
                throw new InvalidOperationException($"{errorMessage} {ex.Message}", ex);
            }        
        }

    }
}
