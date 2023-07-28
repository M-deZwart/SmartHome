using Application.Application.DTOs;
using Application.Application.Exceptions;
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

    public async Task<TemperatureDTO> GetCurrentTemperature()
    {
        var temperature = await _temperatureRepository.GetLatestTemperature();
        var temperatureDTO = _temperatureMapper.MapToDTO(temperature);

        return temperatureDTO;
    }

    public async Task<List<TemperatureDTO>> GetTemperatureByDateRange(DateTime startDate, DateTime endDate)
    {
        var temperatureList = await _temperatureRepository.GetByDateRange(startDate, endDate);
        List<TemperatureDTO> temperatureListDTO = new List<TemperatureDTO>();

        temperatureList.ForEach(temperature =>
        {
            var temperatureDTO = _temperatureMapper.MapToDTO(temperature);
            temperatureListDTO.Add(temperatureDTO);
        });
        return temperatureListDTO;
    }

    public async Task SetTemperature(double celsius)
    {
        Temperature temperature = new Temperature
            (
                celsius: celsius,
                date: DateTime.UtcNow
            );

        await _temperatureRepository.Create(temperature);
    }

}
