using Domain.Domain.Entities;

namespace Domain.Domain.Contracts
{
    public interface IHumidityRepository
    {
        Task Create(Humidity humidity, string sensorTitle);
        Task<Humidity> GetLatestHumidity(string sensorTitle);
        Task<List<Humidity>> GetByDateRange(
            DateTime startDate,
            DateTime endDate,
            string sensorTitle
        );
    }
}
