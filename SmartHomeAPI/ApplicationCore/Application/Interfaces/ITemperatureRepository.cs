using Domain.Domain.Entities;

namespace Application.Application.Interfaces
{
    public interface ITemperatureRepository
    {
        Task Create(Temperature temperature);
        Task<Temperature> GetLatestTemperature();
        Task<List<Temperature>> GetByDateRange(DateTime startDate, DateTime endDate);
    }
}
