using Application.Application.DTOs;
using Application.Application.Exceptions;
using Application.Application.Contracts;
using Domain.Domain.Validators;
using Domain.Domain.Contracts;
using Domain.Domain.Entities;

namespace Application.Application.Services;
public class HumidityService : IHumidityService
{
    private readonly IHumidityRepository _humidityRepository;
    private readonly IHumidityMapper _humidityMapper;
    private readonly IRequestLogger _requestLogger;
    private readonly HumidityValidator _humidityValidator;

    public HumidityService(
        IHumidityRepository humidityRepository,
        IHumidityMapper humidityMapper,
        IRequestLogger requestLogger)
    {
        _humidityRepository = humidityRepository;
        _humidityMapper = humidityMapper;
        _requestLogger = requestLogger;
        _humidityValidator = new HumidityValidator();
    }

    public async Task<HumidityDTO> GetCurrentHumidity()
    {
        try
        {
            var humidity = await _humidityRepository.GetLatestHumidity();

            if (humidity is not null)
            {
                var humidityDTO = _humidityMapper.MapToDTO(humidity);
                _requestLogger.LogRequest(nameof(GetCurrentHumidity), humidityDTO.Percentage, humidityDTO.Date);
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
                _requestLogger.LogRequest(nameof(GetHumidityByDateRange), humidityDTO.Percentage, humidityDTO.Date);
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

            var validationResult = await _humidityValidator.ValidateAsync(humidity);

            if (validationResult.IsValid)
            {
                await _humidityRepository.Create(humidity);
                _requestLogger.LogRequest(nameof(SetHumidity), humidity.Percentage, humidity.Date);
            }
            else
            {
                var validationErrors = validationResult.Errors.Select(error => error.ErrorMessage);
                throw new OutOfRangeException($"Validation errors occurred: {validationErrors}");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failure while setting the humidity: {ex.Message}", ex);
        }
    }

}
