using Domain.Domain.Entities;

namespace Domain.Domain.Contracts
{
    public interface ITemperatureRepository
    {
        Task Create(Temperature temperature, string sensorTitle);
        Task<Temperature> GetLatestTemperature(string sensorTitle);
        Task<List<Temperature>> GetByDateRange(DateTime startDate, DateTime endDate, string sensorTitle);
    }
}
