using Application.Application.Exceptions;
using Domain.Domain.Contracts;
using Domain.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Infrastructure.Repositories
{
    public class HumidityRepositoryEF : IHumidityRepository
    {
        private readonly SmartHomeContext _smartHomeContext;

        public HumidityRepositoryEF(SmartHomeContext smartHomeContext)
        {
            _smartHomeContext = smartHomeContext;
        }

        public async Task Create(Humidity humidity)
        {
            // zomer en wintertijd moet automatisch zijn
            _smartHomeContext.Humidities.Add(humidity);
            await _smartHomeContext.SaveChangesAsync();
        }

        public async Task<List<Humidity>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            var humidityList = await _smartHomeContext.Humidities
            .Where(h => h.Date >= startDate && h.Date <= endDate)
            .ToListAsync();

            return humidityList;
        }

        public async Task<Humidity> GetLatestHumidity()
        {
            var latestHumidity = await _smartHomeContext.Humidities
                .OrderByDescending(h => h.Date)
                .FirstOrDefaultAsync();

            if (latestHumidity is not null)
            {
                return latestHumidity;
            }
            else
            {
                throw new NotFoundException($"Humidity was not found");
            }
        }

    }
}
