using Application.Application.Exceptions;
using Domain.Domain.Contracts;
using Domain.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Infrastructure.Repositories
{
    public class TemperatureRepositoryEF : ITemperatureRepository
    {
        private readonly SmartHomeContext _smartHomeContext;

        public TemperatureRepositoryEF(SmartHomeContext smartHomeContext)
        {
            _smartHomeContext = smartHomeContext;
        }

        public async Task Create(Temperature temperature)
        {
            _smartHomeContext.Temperatures.Add(temperature);
            await _smartHomeContext.SaveChangesAsync();
        }

        public async Task<List<Temperature>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            var temperatureList = await _smartHomeContext.Temperatures
            .Where(t => t.Date >= startDate && t.Date <= endDate)
            .ToListAsync();

            return temperatureList;
        }

        public async Task<Temperature> GetLatestTemperature()
        {
            var latestTemperature = await _smartHomeContext.Temperatures
                .OrderByDescending(t => t.Date)
                .FirstOrDefaultAsync();

            if (latestTemperature is not null)
            {
                return latestTemperature;
            }

            throw new NotFoundException($"Temperature was not found");
        }

    }
}
