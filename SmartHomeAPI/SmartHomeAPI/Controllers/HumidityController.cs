using Application.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Application.Application.Services;

namespace SmartHomeAPI.Controllers
{
    [Route("api/humidity")]
    [ApiController]
    public class HumidityController : ControllerBase
    {
        private readonly IHumidityService _humidityService;

        public HumidityController(IHumidityService humidityService)
        {
            _humidityService = humidityService;
        }

        [HttpGet("{percentage}")]
        public async Task<IActionResult> SetHumidity([FromRoute] double percentage)
        {
            try
            {
                await _humidityService.SetHumidity(percentage);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Humidity could not be set: {ex.Message}");
            }
        }

        [HttpGet("getCurrentHumidity/{id}")]
        public async Task<ActionResult<HumidityDTO>> GetCurrentHumidity(Guid id)
        {
            try
            {
                var humidityDTO = await _humidityService.GetCurrentHumidity(id);
                return Ok(humidityDTO);
            }
            catch (Exception ex)
            {
                return NotFound($"Humidity could not be found: {ex.Message}");
            }
        }

        [HttpGet("humidityByDateRange")]
        public async Task<ActionResult<List<HumidityDTO>>> GetHumidityByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                var humidityListDTO = await _humidityService.GetHumidityByDateRange(startDate, endDate);
                return Ok(humidityListDTO);
            }
            catch (Exception ex)
            {
                return NotFound($"Date range for humidities could not be found: {ex.Message}");
            }
        }

    }
}
