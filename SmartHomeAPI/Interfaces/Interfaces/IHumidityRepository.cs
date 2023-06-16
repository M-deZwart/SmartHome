
using SmartHomeAPI.ApplicationCore.Entities;

namespace Interfaces.Interfaces
{
    public interface IHumidityRepository
    {
        void Create(Humidity humidity);
        List<Humidity> GetByDateRange(DateTime startDate, DateTime endDate);
    }
}
