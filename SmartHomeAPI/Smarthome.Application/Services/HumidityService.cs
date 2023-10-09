using Smarthome.Application.DTOs;
using Smarthome.Domain.Contracts;
using Smarthome.Domain.Entities;

namespace Application.Services;

public class HumidityService : IHumidityService
{
    private readonly IHumidityRepository _humidityRepository;

    public HumidityService(IHumidityRepository humidityRepository)
    {
        _humidityRepository = humidityRepository;
    }

    public async Task<HumidityDTO> GetCurrentHumidity(string sensorTitle)
    {
        var humidity = await _humidityRepository.GetLatestHumidity(sensorTitle);
        var humidityDTO = HumidityDTO.FromDomain(humidity);

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
        var humidityListDTO = new List<HumidityDTO>();

        humidityList.ForEach(humidity =>
        {
            var humidityDTO = HumidityDTO.FromDomain(humidity);
            humidityListDTO.Add(humidityDTO);
        });

        return humidityListDTO;
    }

    public async Task SetHumidity(double percentage, string sensorTitle)
    {
        var humidity = new Humidity(percentage: percentage, date: DateTime.Now);

        await _humidityRepository.Create(humidity, sensorTitle);
    }
}
