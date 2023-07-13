using Domain.Domain.Entities;

namespace Domain.Domain.Contracts
{
    public interface IHumidityRepository
    {
        Task Create(Humidity humidity);
        Task<Humidity> GetLatestHumidity();
        Task<List<Humidity>> GetByDateRange(DateTime startDate, DateTime endDate);
    }
}
