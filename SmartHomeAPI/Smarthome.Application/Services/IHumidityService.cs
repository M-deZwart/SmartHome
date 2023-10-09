using Smarthome.Application.DTOs;

namespace Application.Services;

public interface IHumidityService
{
    Task<HumidityDTO> GetCurrentHumidity(string sensorTitle);
    Task<List<HumidityDTO>> GetHumidityByDateRange(
        DateTime startDate,
        DateTime endDate,
        string sensorTitle
    );
    Task SetHumidity(double percentage, string sensorTitle);
}
