using Application.Application.DTOs;
using Application.Application.Exceptions;
using Application.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Infrastructure.Repositories
{
    public class HumidityRepositoryEF : IHumidityRepository
    {
        private readonly SmartHomeContext _smartHomeContext;
        private readonly ILogger<HumidityRepositoryEF> _logger;

        public HumidityRepositoryEF(
            SmartHomeContext smartHomeContext,
            ILogger<HumidityRepositoryEF> logger)
        {
            _smartHomeContext = smartHomeContext;
            _logger = logger;
        }

        public async Task Create(Humidity humidity)
        {
            try
            {
                humidity.Date = humidity.Date.ToUniversalTime().AddHours(2);
                _smartHomeContext.Humidities.Add(humidity);
                await _smartHomeContext.SaveChangesAsync();
            }
            catch (Exception ex)
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
                var humidityList = await _smartHomeContext.Humidities
                .Where(h => h.Date >= startDate && h.Date <= endDate)
                .ToListAsync();

                return humidityList;
            }
            catch (Exception ex)
            {
                var errorMessage = "Failed to get humidity by date range:";
                _logger.LogError(ex, $"{errorMessage} {ex.Message}");
                throw new InvalidOperationException($"{errorMessage} {ex.Message}", ex);
            }
        }

        public async Task<Humidity> GetByDateTime(DateTime dateTime)
        {
            try
            {
                var humidity = await _smartHomeContext.Humidities.FirstOrDefaultAsync(h => h.Date == dateTime);
                if (humidity is not null)
                {
                    return humidity;
                }
                else
                {
                    throw new NotFoundException($"Humidity with DateTime: {dateTime} could not be found");
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "Failed to get humidity by DateTime:";
                _logger.LogError(ex, $"{errorMessage} {ex.Message}");
                throw new InvalidOperationException($"{errorMessage} {ex.Message}", ex);
            }
        }

    }
}
