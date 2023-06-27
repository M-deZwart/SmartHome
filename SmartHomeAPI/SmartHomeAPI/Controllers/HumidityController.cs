using ApplicationCore.ApplicationCore.DTOs;
using Interfaces.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.ApplicationCore.Entities;
using SmartHomeAPI.Interfaces;
using SmartHomeAPI.MappersAPI;

namespace SmartHomeAPI.Controllers
{
    [Route("api/humidity")]
    [ApiController]
    public class HumidityController : ControllerBase
    {
        private readonly IHumidityRepository _humidityRepository;
        private readonly IHumidityMapper _humidityMapper;
        private readonly IRequestLogger _requestLogger;

        private const string ERROR_OCCURED_MESSAGE = "An error occurred:";

        public HumidityController(
            IHumidityRepository humidityRepository,
            IHumidityMapper humidityMapper,
            IRequestLogger requestLogger
          )
        {
            _humidityRepository = humidityRepository;
            _humidityMapper = humidityMapper;
            _requestLogger = requestLogger;
        }

        [HttpGet("{percentage}")]
        public async Task<IActionResult> SetHumidity([FromRoute] double percentage)
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
                return Ok(humidity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ERROR_OCCURED_MESSAGE} {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HumidityDTO>> GetCurrentHumidity(Guid id)
        {
            try
            {
                var humidity = await _humidityRepository.GetById(id);

                if (humidity is not null)
                {
                    var humidityDTO = _humidityMapper.MapToDTO(humidity);
                    _requestLogger.LogRequest("GetCurrentHumidity", humidityDTO.Percentage, humidityDTO.Date);
                    return Ok(humidityDTO);
                }
                return NotFound("Humidity not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ERROR_OCCURED_MESSAGE} {ex.Message}");
            }
        }

        [HttpGet("humidityByDateRange")]
        public async Task<ActionResult<List<HumidityDTO>>> GetHumidityByDateRange(DateTime startDate, DateTime endDate)
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

                if (humidityListDTO.Count > 0)
                {
                    return Ok(humidityListDTO);
                }
                else
                {
                    return NotFound("No humidity data found for the specified date range");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ERROR_OCCURED_MESSAGE} {ex.Message}");
            }
        }

    }
}
