using Smarthome.Application.DTOs;
using Smarthome.Domain.Contracts;
using Smarthome.Domain.Entities;

namespace Application.Services;

public class TemperatureService : ITemperatureService
{
    private readonly ITemperatureRepository _temperatureRepository;

    public TemperatureService(ITemperatureRepository temperatureRepository)
    {
        _temperatureRepository = temperatureRepository;
    }

    public async Task<TemperatureDTO> GetCurrentTemperature(string sensorTitle)
    {
        var temperature = await _temperatureRepository.GetLatestTemperature(sensorTitle);
        var temperatureDTO = TemperatureDTO.FromDomain(temperature);

        return temperatureDTO;
    }

    public async Task<List<TemperatureDTO>> GetTemperatureByDateRange(
        DateTime startDate,
        DateTime endDate,
        string sensorTitle
    )
    {
        var temperatureList = await _temperatureRepository.GetByDateRange(
            startDate,
            endDate,
            sensorTitle
        );
        var temperatureListDTO = new List<TemperatureDTO>();

        temperatureList.ForEach(temperature =>
        {
            var temperatureDTO = TemperatureDTO.FromDomain(temperature);
            temperatureListDTO.Add(temperatureDTO);
        });
        return temperatureListDTO;
    }

    public async Task SetTemperature(double celsius, string sensorTitle)
    {
        var temperature = new Temperature(celsius: celsius, date: DateTime.Now);

        await _temperatureRepository.Create(temperature, sensorTitle);
    }
}
