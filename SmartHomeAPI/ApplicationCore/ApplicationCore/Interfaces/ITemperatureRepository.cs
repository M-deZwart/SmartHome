using SmartHomeAPI.ApplicationCore.Entities;

namespace SmartHomeAPI.ApplicationCore.Interfaces
{
    public interface ITemperatureRepository
    {
        void Create(Temperature temperature);
        List<Temperature> GetByDateRange(DateTime startDate, DateTime endDate);
    }
}
