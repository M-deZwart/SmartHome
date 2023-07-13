using Application.Application.DTOs;

namespace Application.Application.Interfaces
{
    public interface ITemperatureRepository
    {
        Task Create(Temperature temperature);
        Task<Temperature> GetLatestTemperature();
        Task<List<Temperature>> GetByDateRange(DateTime startDate, DateTime endDate);
    }
}
