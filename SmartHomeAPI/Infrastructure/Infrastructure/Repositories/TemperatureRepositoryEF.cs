using ApplicationCore.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartHomeAPI.ApplicationCore.Entities;

namespace Infrastructure.Infrastructure.Repositories
{
    public class TemperatureRepositoryEF : ITemperatureRepository
    {
        private readonly SmartHomeContext _smartHomeContext;
        private readonly ILogger<TemperatureRepositoryEF> _logger;

        public TemperatureRepositoryEF(
            SmartHomeContext smartHomeContext, 
            ILogger<TemperatureRepositoryEF> logger)
        {
            _smartHomeContext = smartHomeContext;
            _logger = logger;
        }

        public async Task Create(Temperature temperature)
        {
            try
            {              
                temperature.Date = temperature.Date.ToUniversalTime().AddHours(2);
                _smartHomeContext.Temperatures.Add(temperature);
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
                var temperatureList = await _smartHomeContext.Temperatures
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .ToListAsync();          

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
                var temperature = await _smartHomeContext.Temperatures.FirstOrDefaultAsync(t => t.Id == id);

                if (temperature is not null)
                {
                    return temperature;
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
