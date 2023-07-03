using Application.Application.DTOs;
using Application.Application.Interfaces;
using Application.Application.Services;
using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Application.Entities;
using SmartHomeAPI.MappersAPI;

namespace SmartHomeAPI.Controllers
{
    [Route("api/temperature")]
    [ApiController]
    public class TemperatureController : ControllerBase
    {
        private readonly ITemperatureService _temperatureService;

        public TemperatureController(
            ITemperatureService temperatureService
          )
        {
            _temperatureService = temperatureService;
        }

        [HttpGet("{celsius}")]
        public async Task<IActionResult> SetTemperature([FromRoute] double celsius)
        {
            try
            {
                await _temperatureService.SetTemperature(celsius);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Temperature could not be set: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TemperatureDTO>> GetCurrentTemperature(Guid id)
        {
            try
            {
                var temperatureDTO = await _temperatureService.GetCurrentTemperature(id);
                return Ok(temperatureDTO);
            }
            catch (Exception ex)
            {
                return NotFound($"Temperature could not be found: {ex.Message}");
            }
        }

        [HttpGet("temperatureByDateRange")]
        public async Task<ActionResult<List<TemperatureDTO>>> GetTemperatureByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                var humidityListDTO = await _temperatureService.GetTemperatureByDateRange(startDate, endDate);
                return Ok(humidityListDTO);
            }
            catch (Exception ex)
            {
                return NotFound($"Date range for temperatures could not be found: {ex.Message}");
            }
        }

    }
}
