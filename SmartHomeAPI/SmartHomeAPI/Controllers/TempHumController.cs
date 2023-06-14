using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.ApplicationCore.Entities;
using SmartHomeAPI.ApplicationCore.Interfaces;

namespace SmartHomeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class TempHumController : Controller
    {
        private readonly ILogger<TempHumController> _logger;
        private readonly ITemperatureRepository _temperatureRepository;
        private readonly IHumidityRepository _humidityRepository;

        public TempHumController(
            ILogger<TempHumController> logger,
            ITemperatureRepository temperatureRepository,
            IHumidityRepository humidityRepository
            )
        {
            _logger = logger;
            _temperatureRepository = temperatureRepository;
            _humidityRepository = humidityRepository;
        }

        [HttpGet("temperature/{celsius}")]
        public IActionResult SetTemperature([FromRoute] float celsius)
        {
            Temperature temperature;

            temperature = new Temperature
            {
                Celsius = celsius,
                Date = DateTime.Now
            };

            _temperatureRepository.Create(temperature);

            LoqRequest("SetTemperature", temperature.Celsius, temperature.Date);
            return Ok(temperature);
        }

        [HttpGet("temperatureByDateRange")]
        public IActionResult GetTemperatureByDateRange(DateTime startDate, DateTime endDate)
        {
            var temperatureList = _temperatureRepository.GetByDateRange(startDate, endDate);

            temperatureList.ForEach(temperature =>
            {
                LoqRequest("GetTemperatureByDateRange", temperature.Celsius, temperature.Date);
            });
            
            return Ok(temperatureList);
        }

        [HttpGet("humidity/{percentage}")]
        public IActionResult SetHumidity([FromRoute] float percentage)
        {
            Humidity humidity;

            humidity = new Humidity
            {
                Percentage = percentage,
                Date = DateTime.Now
            };

            _humidityRepository.Create(humidity);

            LoqRequest("SetHumidity", humidity.Percentage, humidity.Date);
            return Ok(humidity);
        }

        [HttpGet("humidityByDateRange")]
        public IActionResult GetHumidityByDateRange(DateTime startDate, DateTime endDate)
        {
            var humidityList = _humidityRepository.GetByDateRange(startDate, endDate);

            humidityList.ForEach(humidity =>
            {
                LoqRequest("GetTemperatureByDateRange", humidity.Percentage, humidity.Date);
            });
            
            return Ok(humidityList);
        }

        private void LoqRequest(string action, object result, DateTime timestamp)
        {
            var request = $"{HttpContext.Request.Method} {HttpContext.Request.Path}";
            var requestBody = HttpContext.Request.QueryString.Value;
            var responseBody = result.ToString();

            _logger.LogInformation($"Request: {request} {requestBody}");
            _logger.LogInformation($"Response: {responseBody}");
            _logger.LogInformation($"Timestamp: {timestamp}");
        }
    }
}
