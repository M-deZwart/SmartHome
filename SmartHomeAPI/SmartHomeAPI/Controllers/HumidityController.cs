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

        [HttpPost("setHumidity")]
        public async Task<IActionResult> SetHumidity([FromBody] double percentage)
        {
            try
            {
                var validationResult = await _percentageValidator.ValidateAsync(percentage);

                if (validationResult.IsValid)
                {
                    await _humidityService.SetHumidity(percentage);
                    return Ok();
                }
                else
                {
                    var validationErrors = validationResult.Errors.Select(error => error.ErrorMessage);
                    throw new OutOfRangeException($"Validation errors occurred: {validationErrors}");
                }
                
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
       
        [HttpGet("humidityByDateRange")]
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
