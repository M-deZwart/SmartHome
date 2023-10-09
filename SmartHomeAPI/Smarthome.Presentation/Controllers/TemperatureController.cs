using Smarthome.Application.DTOs;
using Smarthome.Application.Exceptions;
using Application.Services;
using Smarthome.Application.Validators;
using Microsoft.AspNetCore.Mvc;

namespace Smarthome.Presentation.Controllers
{
    [Route("api/temperature")]
    [ApiController]
    public class TemperatureController : ControllerBase
    {
        private readonly ITemperatureService _temperatureService;
        private readonly CelsiusValidator _celsiusValidator;

        public TemperatureController(ITemperatureService temperatureService)
        {
            _temperatureService = temperatureService;
            _celsiusValidator = new CelsiusValidator();
        }

        [HttpPost("setTemperature/{sensorTitle}")]
        public async Task<IActionResult> SetTemperature(
            [FromBody] double celsius,
            string sensorTitle
        )
        {
            try
            {
                var validationResult = await _celsiusValidator.ValidateAsync(celsius);

                if (validationResult.IsValid)
                {
                    await _temperatureService.SetTemperature(celsius, sensorTitle);
                    return Ok();
                }
                else
                {
                    var validationErrors = validationResult.Errors.Select(
                        error => error.ErrorMessage
                    );
                    throw new OutOfRangeException(
                        $"Validation errors occurred: {validationErrors}"
                    );
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Temperature could not be set: {ex.Message}");
            }
        }

        [HttpGet("getCurrentTemperature/{sensorTitle}")]
        public async Task<ActionResult<TemperatureDTO>> GetCurrentTemperature(string sensorTitle)
        {
            try
            {
                var temperatureDTO = await _temperatureService.GetCurrentTemperature(sensorTitle);
                return Ok(temperatureDTO);
            }
            catch (Exception ex)
            {
                return NotFound($"Temperature could not be found: {ex.Message}");
            }
        }

        [HttpGet("temperatureByDateRange/{sensorTitle}")]
        public async Task<ActionResult<List<TemperatureDTO>>> GetTemperatureByDateRange(
            DateTime startDate,
            DateTime endDate,
            string sensorTitle
        )
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(
                        "Invalid date range. The start date must be less than the end date."
                    );
                }

                var temperatureListDTO = await _temperatureService.GetTemperatureByDateRange(
                    startDate,
                    endDate,
                    sensorTitle
                );
                return Ok(temperatureListDTO);
            }
            catch (Exception ex)
            {
                return NotFound($"Date range for temperatures could not be found: {ex.Message}");
            }
        }
    }
}
