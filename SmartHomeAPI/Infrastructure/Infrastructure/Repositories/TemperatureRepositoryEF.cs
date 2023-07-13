using Application.Application.Interfaces;
using Domain.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        public async Task<Temperature> GetLatestTemperature()
        {
            try
            {
                var latestTemperature = await _smartHomeContext.Temperatures
                    .OrderByDescending(t => t.Date)
                    .FirstOrDefaultAsync();

                if (latestTemperature is not null)
                {
                    return latestTemperature;
                }

                throw new InvalidOperationException($"Temperature was not found");
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
