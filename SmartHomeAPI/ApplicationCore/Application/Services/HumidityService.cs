using Application.Application.DTOs;
using Application.Application.Exceptions;
using Application.Application.Interfaces;
using Interfaces.Interfaces;
using SmartHomeAPI.Application.Entities;
using SmartHomeAPI.MappersAPI;

namespace Application.Application.Services;
public class HumidityService : IHumidityService
{
    private readonly IHumidityRepository _humidityRepository;
    private readonly IHumidityMapper _humidityMapper;
    private readonly IRequestLogger _requestLogger;

    public HumidityService(
        IHumidityRepository humidityRepository,
        IHumidityMapper humidityMapper,
        IRequestLogger requestLogger)
    {
        _humidityRepository = humidityRepository;
        _humidityMapper = humidityMapper;
        _requestLogger = requestLogger;
    }

    public async Task<HumidityDTO> GetCurrentHumidity(Guid id)
    {
        try
        {
            var humidity = await _humidityRepository.GetById(id);

            if (humidity is not null)
            {
                var humidityDTO = _humidityMapper.MapToDTO(humidity);
                _requestLogger.LogRequest("GetCurrentHumidity", humidityDTO.Percentage, humidityDTO.Date);
                return humidityDTO;
            }
            else
            {
                throw new NotFoundException("$Humidity was not found");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Humidity could not be retrieved: {ex.Message}", ex);
        }
    }

    public async Task<List<HumidityDTO>> GetHumidityByDateRange(DateTime startDate, DateTime endDate)
    {
        try
        {
            var humidityList = await _humidityRepository.GetByDateRange(startDate, endDate);
            List<HumidityDTO> humidityListDTO = new List<HumidityDTO>();

            humidityList.ForEach(humidity =>
            {
                var humidityDTO = _humidityMapper.MapToDTO(humidity);
                humidityListDTO.Add(humidityDTO);
                _requestLogger.LogRequest("GetHumidityByDateRange", humidityDTO.Percentage, humidityDTO.Date);
            });
            return humidityListDTO;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"The humidities for the date range could not be retrieved: {ex.Message}", ex);
        }
    }

    public async Task SetHumidity(double percentage)
    {
        try
        {
            Humidity humidity = new Humidity
            {
                Percentage = percentage,
                Date = DateTime.Now
            };

            await _humidityRepository.Create(humidity);

            _requestLogger.LogRequest("SetHumidity", humidity.Percentage, humidity.Date);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failure while setting the humidity: {ex.Message}", ex);
        }
    }
}
