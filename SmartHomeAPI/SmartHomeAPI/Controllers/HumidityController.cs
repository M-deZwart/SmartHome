using Application.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Application.Application.Services;
using Application.Application.Validators;
using Application.Application.Exceptions;

namespace SmartHomeAPI.Controllers
{
    [Route("api/humidity")]
    [ApiController]
    public class HumidityController : ControllerBase
    {
        private readonly IHumidityService _humidityService;
        private readonly PercentageValidator _percentageValidator;

        public HumidityController(IHumidityService humidityService)
        {
            _humidityService = humidityService;
            _percentageValidator = new PercentageValidator();
        }

        [HttpPost("setHumidity/{sensorTitle}")]
        public async Task<IActionResult> SetHumidity(
            [FromBody] double percentage,
            string sensorTitle
        )
        {
            try
            {
                var validationResult = await _percentageValidator.ValidateAsync(percentage);

                if (validationResult.IsValid)
                {
                    await _humidityService.SetHumidity(percentage, sensorTitle);
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
                return BadRequest($"Humidity could not be set: {ex.Message}");
            }
        }

        [HttpGet("getCurrentHumidity/{sensorTitle}")]
        public async Task<ActionResult<HumidityDTO>> GetCurrentHumidity(string sensorTitle)
        {
            try
            {
                var humidityDTO = await _humidityService.GetCurrentHumidity(sensorTitle);
                return Ok(humidityDTO);
            }
            catch (Exception ex)
            {
                return NotFound($"Humidity could not be found: {ex.Message}");
            }
        }

        [HttpGet("humidityByDateRange/{sensorTitle}")]
        public async Task<ActionResult<List<HumidityDTO>>> GetHumidityByDateRange(
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

                var humidityListDTO = await _humidityService.GetHumidityByDateRange(
                    startDate,
                    endDate,
                    sensorTitle
                );
                return Ok(humidityListDTO);
            }
            catch (Exception ex)
            {
                return NotFound($"Date range for humidities could not be found: {ex.Message}");
            }
        }
    }
}
