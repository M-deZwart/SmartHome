using Application.Application.DTOs;
using Interfaces.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Application.Entities;
using Application.Application.Interfaces;
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
            Humidity humidity = new Humidity
            {
                Percentage = percentage,
                Date = DateTime.Now
            };

            await _humidityRepository.Create(humidity);

            _requestLogger.LogRequest("SetHumidity", humidity.Percentage, humidity.Date);
            return Ok(humidity);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HumidityDTO>> GetCurrentHumidity(Guid id)
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

        [HttpGet("humidityByDateRange")]
        public async Task<ActionResult<List<HumidityDTO>>> GetHumidityByDateRange(DateTime startDate, DateTime endDate)
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
            return NotFound("No humidity data found for the specified date range");
        }

    }
}
