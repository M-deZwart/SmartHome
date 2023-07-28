using Application.Application.DTOs;
using Application.Application.Contracts;
using Domain.Domain.Contracts;
using Domain.Domain.Entities;

namespace Application.Application.Services;
public class HumidityService : IHumidityService
{
    private readonly IHumidityRepository _humidityRepository;
    private readonly IHumidityMapper _humidityMapper;

    public HumidityService(
        IHumidityRepository humidityRepository,
        IHumidityMapper humidityMapper)
    {
        _humidityRepository = humidityRepository;
        _humidityMapper = humidityMapper;
    }

    public async Task<HumidityDTO> GetCurrentHumidity()
    {
        var humidity = await _humidityRepository.GetLatestHumidity();
        var humidityDTO = _humidityMapper.MapToDTO(humidity);

        return humidityDTO;
    }

    public async Task<List<HumidityDTO>> GetHumidityByDateRange(DateTime startDate, DateTime endDate)
    {
        var humidityList = await _humidityRepository.GetByDateRange(startDate, endDate);
        List<HumidityDTO> humidityListDTO = new List<HumidityDTO>();

        humidityList.ForEach(humidity =>
        {
            var humidityDTO = _humidityMapper.MapToDTO(humidity);
            humidityListDTO.Add(humidityDTO);
        });

        return humidityListDTO;
    }

    public async Task SetHumidity(double percentage)
    {
        Humidity humidity = new Humidity
        (
            percentage: percentage,
            date: DateTime.Now
        );

        await _humidityRepository.Create(humidity);
    }

}
