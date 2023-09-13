using Application.Application.DTOs;
using Application.Application.Contracts;
using Domain.Domain.Contracts;
using Domain.Domain.Entities;

namespace Application.Application.Services;
public class TemperatureService : ITemperatureService
{
    private readonly ITemperatureRepository _temperatureRepository;
    private readonly ITemperatureMapper _temperatureMapper;

    public TemperatureService(
        ITemperatureRepository temperatureRepository,
        ITemperatureMapper temperatureMapper)
    {
        _temperatureRepository = temperatureRepository;
        _temperatureMapper = temperatureMapper;
    }

    public async Task<TemperatureDTO> GetCurrentTemperature(string sensorTitle)
    {
        var temperature = await _temperatureRepository.GetLatestTemperature(sensorTitle);
        var temperatureDTO = _temperatureMapper.MapToDTO(temperature);

        return temperatureDTO;
    }

    public async Task<List<TemperatureDTO>> GetTemperatureByDateRange(DateTime startDate, DateTime endDate, string sensorTitle)
    {
        var temperatureList = await _temperatureRepository.GetByDateRange(startDate, endDate, sensorTitle);
        List<TemperatureDTO> temperatureListDTO = new List<TemperatureDTO>();

        temperatureList.ForEach(temperature =>
        {
            var temperatureDTO = _temperatureMapper.MapToDTO(temperature);
            temperatureListDTO.Add(temperatureDTO);
        });
        return temperatureListDTO;
    }

    public async Task SetTemperature(double celsius, string sensorTitle)
    {
        Temperature temperature = new Temperature
        (
            celsius: celsius,
            date: DateTime.Now
        );

        await _temperatureRepository.Create(temperature, sensorTitle);
    }

}
