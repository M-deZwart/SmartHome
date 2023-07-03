using Application.Application.DTOs;
using Application.Application.Exceptions;
using Application.Application.Interfaces;

namespace Application.Application.Services;
public class TemperatureService : ITemperatureService
{
    private readonly ITemperatureRepository _temperatureRepository;
    private readonly ITemperatureMapper _temperatureMapper;
    private readonly IRequestLogger _requestLogger;

    public TemperatureService(
        ITemperatureRepository temperatureRepository,
        ITemperatureMapper temperatureMapper,
        IRequestLogger requestLogger)
    {
        _temperatureRepository = temperatureRepository;
        _temperatureMapper = temperatureMapper;
        _requestLogger = requestLogger;
    }

    public async Task<TemperatureDTO> GetCurrentTemperature(Guid id)
    {
        try
        {
            var temperature = await _temperatureRepository.GetById(id);

            if (temperature is not null)
            {
                var temperatureDTO = _temperatureMapper.MapToDTO(temperature);
                _requestLogger.LogRequest("GetCurrentTemperature", temperatureDTO.Celsius, temperatureDTO.Date);
                return temperatureDTO;
            }
            else
            {
                throw new NotFoundException("$Temperature was not found");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Temperature could not be retrieved: {ex.Message}", ex);
        }
    }

    public async Task<List<TemperatureDTO>> GetTemperatureByDateRange(DateTime startDate, DateTime endDate)
    {
        try
        {
            var temperatureList = await _temperatureRepository.GetByDateRange(startDate, endDate);
            List<TemperatureDTO> temperatureListDTO = new List<TemperatureDTO>();

            temperatureList.ForEach(temperature =>
            {
                var temperatureDTO = _temperatureMapper.MapToDTO(temperature);
                temperatureListDTO.Add(temperatureDTO);
                _requestLogger.LogRequest("GetTemperatureByDateRange", temperatureDTO.Celsius, temperatureDTO.Date);
            });
            return temperatureListDTO;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"The temperatures for the date range could not be retrieved: {ex.Message}", ex);
        }
    }

    public async Task SetTemperature(double celsius)
    {
        try
        {
            Temperature temperature = new Temperature
            {
                Celsius = celsius,
                Date = DateTime.Now
            };

            await _temperatureRepository.Create(temperature);

            _requestLogger.LogRequest("SetTemperature", temperature.Celsius, temperature.Date);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failure while setting the temperature: {ex.Message}", ex);
        }
    }
}
