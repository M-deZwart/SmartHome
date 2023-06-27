using ApplicationCore.ApplicationCore.Interfaces.InfraMappers;
using Infrastructure.Infrastructure.DTOs;
using Interfaces.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartHomeAPI.ApplicationCore.Entities;
using System.Linq.Expressions;

namespace Infrastructure.Infrastructure.Repositories
{
    public class HumidityRepositoryEF : IHumidityRepository
    {
        private readonly SmartHomeContext _smartHomeContext;
        private readonly IHumidityMapper<HumidityEfDTO> _humidityMapper;
        private readonly ILogger<HumidityRepositoryEF> _logger;

        public HumidityRepositoryEF(
            SmartHomeContext smartHomeContext, 
            IHumidityMapper<HumidityEfDTO> humidityMapper, 
            ILogger<HumidityRepositoryEF> logger) 
        {
            _smartHomeContext = smartHomeContext;
            _humidityMapper = humidityMapper;
            _logger = logger;
        }

        public async Task Create(Humidity humidity)
        {     
            try
            {
                var humidityDTO = _humidityMapper.MapToDTO(humidity);
                humidityDTO.Date.ToUniversalTime().AddHours(2);
                _smartHomeContext.Humidities.Add(humidityDTO);
                await _smartHomeContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var errorMessage = "Failed to create humidity:";
                _logger.LogError(ex, $"{errorMessage} { ex.Message}");
                throw new InvalidOperationException($"{errorMessage} {ex.Message}", ex);
            }      
        }

        public async Task<List<Humidity>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                var humidityListDTO = await _smartHomeContext.Humidities
                .Where(h => h.Date >= startDate && h.Date <= endDate)
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
                var humidityDTO = await _smartHomeContext.Humidities.FirstOrDefaultAsync(h => h.ID == id);

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
