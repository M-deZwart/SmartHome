using SmartHomeAPI.Application.Entities;

namespace Interfaces.Interfaces
{
    public interface IHumidityRepository
    {
        Task Create(Humidity humidity);
        Task<Humidity> GetById(Guid id);
        Task<List<Humidity>> GetByDateRange(DateTime startDate, DateTime endDate);
    }
}
