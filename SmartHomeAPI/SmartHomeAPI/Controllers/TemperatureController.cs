using ApplicationCore.ApplicationCore.DTOs;
using ApplicationCore.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.ApplicationCore.Entities;
using SmartHomeAPI.Interfaces;
using SmartHomeAPI.MappersAPI;

namespace SmartHomeAPI.Controllers
{
    [Route("api/temperature")]
    [ApiController]
    public class TemperatureController : ControllerBase
    {
        private readonly ITemperatureRepository _temperatureRepository;
        private readonly ITemperatureMapper _temperatureMapper;
        private readonly IRequestLogger _requestLogger;

        public TemperatureController(
            ITemperatureRepository temperatureRepository,
            ITemperatureMapper temperatureMapper,
            IRequestLogger requestLogger
          )
        {
            _temperatureRepository = temperatureRepository;
            _temperatureMapper = temperatureMapper;
            _requestLogger = requestLogger;
        }

        [HttpGet("{celsius}")]
        public IActionResult SetTemperature([FromRoute] float celsius)
        {
            try
            {
                Temperature temperature = new Temperature
                {
                    Celsius = celsius,
                    Date = DateTime.Now
                };

                _temperatureRepository.Create(temperature);

                _requestLogger.LogRequest("SetTemperature", temperature.Celsius, temperature.Date);
                return Ok(temperature);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<TemperatureDTO> GetCurrentTemperature(Guid id)
        {
            try
            {
                var temperature = _temperatureRepository.GetById(id);

                if (temperature is not null)
                {
                    var temperatureDTO = _temperatureMapper.MapToDTO(temperature);
                    _requestLogger.LogRequest("GetCurrentTemperature", temperatureDTO.Celsius, temperatureDTO.Date);
                    return Ok(temperatureDTO);
                }
                return NotFound("Temperature not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("temperatureByDateRange")]
        public ActionResult<List<TemperatureDTO>> GetTemperatureByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                var temperatureList = _temperatureRepository.GetByDateRange(startDate, endDate);
                List<TemperatureDTO> temperatureListDTO = new List<TemperatureDTO>();

                temperatureList.ForEach(temperature =>
                {
                    var temperatureDTO = _temperatureMapper.MapToDTO(temperature);
                    temperatureListDTO.Add(temperatureDTO);
                    _requestLogger.LogRequest("GetTemperatureByDateRange", temperatureDTO.Celsius, temperatureDTO.Date);
                });

                if (temperatureListDTO.Count > 0)
                {
                    return Ok(temperatureListDTO);
                }
                else
                {
                    return NotFound("No temperature data found for the specified date range");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
