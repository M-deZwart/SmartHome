using Domain.Domain.Entities;

namespace Domain.Domain.Contracts
{
    public interface ITemperatureRepository
    {
        Task Create(Temperature temperature);
        Task<Temperature> GetLatestTemperature();
        Task<List<Temperature>> GetByDateRange(DateTime startDate, DateTime endDate);
    }
}
