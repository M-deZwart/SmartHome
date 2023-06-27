using ApplicationCore.ApplicationCore.Interfaces;
using ApplicationCore.ApplicationCore.Interfaces.InfraMappers;
using Infrastructure.Infrastructure.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartHomeAPI.ApplicationCore.Entities;

namespace Infrastructure.Infrastructure.Repositories
{
    public class TemperatureRepositoryEF : ITemperatureRepository
    {
        private readonly SmartHomeContext _smartHomeContext;
        private readonly ITemperatureMapper<TemperatureEfDTO> _temperatureMapper;
        private readonly ILogger<TemperatureRepositoryEF> _logger;

        public TemperatureRepositoryEF(
            SmartHomeContext smartHomeContext, 
            ITemperatureMapper<TemperatureEfDTO> temperatureMapper,
            ILogger<TemperatureRepositoryEF> logger)
        {
            _smartHomeContext = smartHomeContext;
            _temperatureMapper = temperatureMapper;
            _logger = logger;
        }

        public async Task Create(Temperature temperature)
        {
            try
            {
                var temperatureDTO = _temperatureMapper.MapToDTO(temperature);
                temperatureDTO.Date = temperature.Date.ToUniversalTime().AddHours(2);
                _smartHomeContext.Temperatures.Add(temperatureDTO);
                await _smartHomeContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var errorMessage = "Failed to create temperature:";
                _logger.LogError(ex, $"{errorMessage} {ex.Message}");
                throw new InvalidOperationException($"{errorMessage} {ex.Message}", ex);
            }
        }

        public async Task <List<Temperature>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                var temperatureListDTO = await _smartHomeContext.Temperatures
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .ToListAsync();
                List<Temperature> temperatureList = new List<Temperature>();

                temperatureListDTO.ForEach(temperatureDTO =>
                {
                    var temperature = _temperatureMapper.MapToEntity(temperatureDTO);
                    temperatureList.Add(temperature);
                });

                return temperatureList;
            }
            catch (Exception ex)
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
                var temperatureDTO = await _smartHomeContext.Temperatures.FirstOrDefaultAsync(t => t.ID == id);

                if (temperatureDTO is not null)
                {
                    return _temperatureMapper.MapToEntity(temperatureDTO);
                }

                throw new InvalidOperationException("Temperature not found");
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
