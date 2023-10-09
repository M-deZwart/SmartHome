using Smarthome.Application.DTOs;
using Smarthome.Application.Contracts;
using Smarthome.Domain.Contracts;
using Smarthome.Domain.Entities;

namespace Application.Services;

public class HumidityService : IHumidityService
{
    private readonly IHumidityRepository _humidityRepository;
    private readonly IHumidityMapper _humidityMapper;

    public HumidityService(IHumidityRepository humidityRepository, IHumidityMapper humidityMapper)
    {
        _humidityRepository = humidityRepository;
        _humidityMapper = humidityMapper;
    }

    public async Task<HumidityDTO> GetCurrentHumidity(string sensorTitle)
    {
        var humidity = await _humidityRepository.GetLatestHumidity(sensorTitle);
        var humidityDTO = _humidityMapper.MapToDTO(humidity);

        return humidityDTO;
    }

    public async Task<List<HumidityDTO>> GetHumidityByDateRange(
        DateTime startDate,
        DateTime endDate,
        string sensorTitle
    )
    {
        var humidityList = await _humidityRepository.GetByDateRange(
            startDate,
            endDate,
            sensorTitle
        );
        List<HumidityDTO> humidityListDTO = new List<HumidityDTO>();

        humidityList.ForEach(humidity =>
        {
            var humidityDTO = _humidityMapper.MapToDTO(humidity);
            humidityListDTO.Add(humidityDTO);
        });

        return humidityListDTO;
    }

    public async Task SetHumidity(double percentage, string sensorTitle)
    {
        Humidity humidity = new Humidity(percentage: percentage, date: DateTime.Now);

        await _humidityRepository.Create(humidity, sensorTitle);
    }
}
