using ApplicationCore.ApplicationCore.Interfaces.InfraMappers;
using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.ApplicationCore.Entities;
using SmartHomeAPI.DTOs;
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
            Temperature temperature;

            temperature = new Temperature
            {
                Celsius = celsius,
                Date = DateTime.Now
            };

            _temperatureRepository.Create(temperature);

            _requestLogger.LogRequest("SetTemperature", temperature.Celsius, temperature.Date);
            return Ok(temperature);
        }

        [HttpGet("{id}")]
        public ActionResult<TemperatureDTO> GetCurrentTemperature(Guid id)
        {
            try
            {
                var temperature = _temperatureRepository.GetById(id);

                if (temperature != null)
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
        public IActionResult GetTemperatureByDateRange(DateTime startDate, DateTime endDate)
        {
            var temperatureList = _temperatureRepository.GetByDateRange(startDate, endDate);
            List<TemperatureDTO> temperatureListDTO = new List<TemperatureDTO>();

            temperatureList.ForEach(temperature =>
            {
                var temperatureDTO = _temperatureMapper.MapToDTO(temperature);
                temperatureListDTO.Add(temperatureDTO);
                _requestLogger.LogRequest("GetTemperatureByDateRange", temperatureDTO.Celsius, temperatureDTO.Date);
            });

            return Ok(temperatureListDTO);
        }

    }
}
