
using SmartHomeAPI.ApplicationCore.Entities;

namespace Interfaces.Interfaces
{
    public interface IHumidityRepository
    {
        void Create(Humidity humidity);
        public Humidity GetById(Guid id);
        List<Humidity> GetByDateRange(DateTime startDate, DateTime endDate);
    }
}
