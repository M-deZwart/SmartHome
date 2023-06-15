using SmartHomeAPI.ApplicationCore.Entities;
using SmartHomeAPI.ApplicationCore.Interfaces;

namespace Infrastructure.Infrastructure
{
    public class TemperatureRepositoryEF : ITemperatureRepository
    {
        private readonly SmartHomeContext _smartHomeContext;

        public TemperatureRepositoryEF(SmartHomeContext smartHomeContext)
        {
            _smartHomeContext = smartHomeContext;
        }

        public void Create(Temperature temperature)
        {
            temperature.Date = temperature.Date.ToUniversalTime().AddHours(2);
            _smartHomeContext.Temperatures.Add(temperature);
            _smartHomeContext.SaveChanges();
        }

        public List<Temperature> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            var temperatureList = _smartHomeContext.Temperatures
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .ToList();

            return temperatureList;
        }
    }
}
