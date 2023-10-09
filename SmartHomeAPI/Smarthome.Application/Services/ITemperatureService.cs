using Smarthome.Application.DTOs;

namespace Application.Services;

public interface ITemperatureService
{
    Task<TemperatureDTO> GetCurrentTemperature(string sensorTitle);
    Task<List<TemperatureDTO>> GetTemperatureByDateRange(
        DateTime startDate,
        DateTime endDate,
        string sensorTitle
    );
    Task SetTemperature(double celsius, string sensorTitle);
}
