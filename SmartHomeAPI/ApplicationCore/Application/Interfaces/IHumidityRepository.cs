using Domain.Domain.Entities;

namespace Application.Application.Interfaces
{
    public interface IHumidityRepository
    {
        Task Create(Humidity humidity);
        Task<Humidity> GetLatestHumidity();
        Task<List<Humidity>> GetByDateRange(DateTime startDate, DateTime endDate);
    }
}
