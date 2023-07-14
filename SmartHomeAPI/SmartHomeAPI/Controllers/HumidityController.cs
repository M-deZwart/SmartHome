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

        [HttpPost("setHumidity")]
        public async Task<IActionResult> SetHumidity([FromBody] double percentage)
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

        [HttpGet("getCurrentHumidity")]
        public async Task<ActionResult<HumidityDTO>> GetCurrentHumidity()
        {
            try
            {
                var humidityDTO = await _humidityService.GetCurrentHumidity();
                return Ok(humidityDTO);
            }
            catch (Exception ex)
            {
                return NotFound($"Humidity could not be found: {ex.Message}");
            }
        }

        [HttpGet("humidityByDateRange/{startDate}/{endDate}")]
        public async Task<ActionResult<List<HumidityDTO>>> GetHumidityByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest("Invalid date range. The start date must be less than the end date.");
                }

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
