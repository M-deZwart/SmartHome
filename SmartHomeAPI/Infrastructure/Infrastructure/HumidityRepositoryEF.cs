using SmartHomeAPI.ApplicationCore.Entities;
using SmartHomeAPI.ApplicationCore.Interfaces;

namespace Infrastructure.Infrastructure
{
    public class HumidityRepositoryEF : IHumidityRepository
    {
        private readonly SmartHomeContext _smartHomeContext;

        public HumidityRepositoryEF(SmartHomeContext smartHomeContext) {
            _smartHomeContext = smartHomeContext;
        }

        public void Create(Humidity humidity)
        {
            humidity.Date.ToUniversalTime().AddHours(2);
            _smartHomeContext.Humidities.Add(humidity);
            _smartHomeContext.SaveChanges();
        }

        public List<Humidity> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            var humidityList = _smartHomeContext.Humidities
                .Where(h => h.Date >= startDate && h.Date <= endDate)
                .ToList();

            return humidityList;
        }
    }
}
