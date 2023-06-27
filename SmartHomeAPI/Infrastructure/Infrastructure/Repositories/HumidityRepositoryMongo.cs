using ApplicationCore.ApplicationCore.Interfaces.InfraMappers;
using Infrastructure.Infrastructure.DTOs;
using Interfaces.Interfaces;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SmartHomeAPI.ApplicationCore.Entities;
using System.Linq;

namespace SmartHomeAPI.Infrastructure.Repositories
{
    public class HumidityRepositoryMongo : IHumidityRepository
    {
        private readonly IHumidityMapper<HumidityMongoDTO> _humidityMapper;
        private readonly IMongoCollection<HumidityMongoDTO> _humidityCollection;
        private readonly ILogger<HumidityRepositoryMongo> _logger;

        public HumidityRepositoryMongo(
            IMongoDatabase db, 
            IHumidityMapper<HumidityMongoDTO> humidityMapper,
            ILogger<HumidityRepositoryMongo> logger)
        {
            _humidityCollection = db.GetCollection<HumidityMongoDTO>("Humidity");
            _humidityMapper = humidityMapper;
            _logger = logger;
        }

        public async Task Create(Humidity humidity)
        {  
            try
            {
                var humidityDTO = _humidityMapper.MapToDTO(humidity);
                humidityDTO.Date = humidity.Date.ToUniversalTime().AddHours(2);
                await _humidityCollection.InsertOneAsync(humidityDTO);
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
                var filter = Builders<HumidityMongoDTO>.Filter.And(
                    Builders<HumidityMongoDTO>.Filter.Gte(h => h.Date, startDate),
                    Builders<HumidityMongoDTO>.Filter.Lte(h => h.Date, endDate)
                );

                var humidityListDTO = await _humidityCollection
                    .Find(filter)
                    .ToListAsync();

                List<Humidity> humidityList = new List<Humidity>();

                humidityListDTO.ForEach(humidityDTO =>
                {
                    var humidity = _humidityMapper.MapToEntity(humidityDTO);
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
                var filter = Builders<HumidityMongoDTO>.Filter.Eq(h => h.ID, id);
                var humidityDTO = await _humidityCollection.Find(filter).FirstOrDefaultAsync();

                if (humidityDTO is not null)
                {
                    return _humidityMapper.MapToEntity(humidityDTO);
                }
                else
                {
                    throw new InvalidOperationException("Humidity not found");
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
