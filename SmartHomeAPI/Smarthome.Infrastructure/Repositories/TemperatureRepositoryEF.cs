using Smarthome.Application.Exceptions;
using Smarthome.Domain.Contracts;
using Smarthome.Domain.Entities;
using Smarthome.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace Smarthome.Infrastructure.Repositories
{
    public class TemperatureRepositoryEF : ITemperatureRepository
    {
        private readonly SmartHomeContext _smartHomeContext;

        public TemperatureRepositoryEF(SmartHomeContext smartHomeContext)
        {
            _smartHomeContext = smartHomeContext;
        }

        public async Task Create(Temperature temperature, string sensorTitle)
        {
            var sensor = await FindSensor(sensorTitle);
            temperature.SensorId = sensor.Id;

            _smartHomeContext.Temperatures.Add(temperature);
            await _smartHomeContext.SaveChangesAsync();
        }

        public async Task<List<Temperature>> GetByDateRange(
            DateTime startDate,
            DateTime endDate,
            string sensorTitle
        )
        {
            var sensor = await FindSensor(sensorTitle);

            var temperatureList = await _smartHomeContext.Temperatures
                .Where(t => t.Date >= startDate && t.Date <= endDate && t.SensorId == sensor.Id)
                .ToListAsync();

            return temperatureList;
        }

        public async Task<Temperature> GetLatestTemperature(string sensorTitle)
        {
            var sensor = await FindSensor(sensorTitle);

            var latestTemperature = await _smartHomeContext.Temperatures
                .Where(t => t.SensorId == sensor.Id)
                .OrderByDescending(t => t.Date)
                .FirstOrDefaultAsync();

            if (latestTemperature is not null)
            {
                return latestTemperature;
            }
            else
            {
                throw new NotFoundException($"Temperature was not found");
            }
        }

        private async Task<Sensor> FindSensor(string sensorTitle)
        {
            var sensor = await _smartHomeContext.Sensors.FirstOrDefaultAsync(
                s => s.Title == sensorTitle
            );

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
