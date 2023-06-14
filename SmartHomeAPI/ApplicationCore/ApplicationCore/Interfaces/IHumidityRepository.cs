using SmartHomeAPI.ApplicationCore.Entities;

namespace SmartHomeAPI.ApplicationCore.Interfaces
{
    public interface IHumidityRepository
    {
        void Create(Humidity humidity);
        List<Humidity> GetByDateRange(DateTime startDate, DateTime endDate);
    }
}
