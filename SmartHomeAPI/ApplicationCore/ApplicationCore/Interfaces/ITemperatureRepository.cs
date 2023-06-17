using SmartHomeAPI.ApplicationCore.Entities;

namespace ApplicationCore.ApplicationCore.Interfaces
{
    public interface ITemperatureRepository
    {
        void Create(Temperature temperature);
        public Temperature GetById(Guid id);
        List<Temperature> GetByDateRange(DateTime startDate, DateTime endDate);
    }
}
