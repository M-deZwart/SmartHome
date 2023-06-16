using SmartHomeAPI.ApplicationCore.Entities;

namespace Interfaces.Interfaces
{
    public interface ITemperatureRepository
    {
        void Create(Temperature temperature);
        List<Temperature> GetByDateRange(DateTime startDate, DateTime endDate);
    }
}
