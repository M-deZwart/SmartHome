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

        public async Task Create(Humidity humidity, string sensorTitle)
        {
            var sensor = await FindSensor(sensorTitle);
            humidity.SensorId = sensor.Id; 

            _smartHomeContext.Humidities.Add(humidity);
            await _smartHomeContext.SaveChangesAsync();
        }

        public async Task<List<Humidity>> GetByDateRange(DateTime startDate, DateTime endDate, string sensorTitle)
        {
            var sensor = await FindSensor(sensorTitle);

            var humidityList = await _smartHomeContext.Humidities
            .Where(h => h.Date >= startDate && h.Date <= endDate && h.SensorId == sensor.Id)
            .ToListAsync();

            return humidityList;
        }

        public async Task<Humidity> GetLatestHumidity(string sensorTitle)
        {
            var sensor = await FindSensor(sensorTitle);

            var latestHumidity = await _smartHomeContext.Humidities
                .Where(h => h.SensorId == sensor.Id)
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

        private async Task<Sensor> FindSensor(string sensorTitle)
        {
            var sensor = await _smartHomeContext.Sensors.FirstOrDefaultAsync(s => s.Title == sensorTitle);

            if (sensor is not null)
            {
                return sensor;
            }
            else
            {
                throw new NotFoundException("Sensor is not found");
            }
        }

    }
}
