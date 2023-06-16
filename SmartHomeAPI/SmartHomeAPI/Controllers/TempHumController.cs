using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.ApplicationCore.Entities;
using SmartHomeAPI.ApplicationCore.Interfaces;
using SmartHomeAPI.DTOs;
using SmartHomeAPI.Interfaces;

namespace SmartHomeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class TempHumController : Controller
    {
        private readonly ILogger<TempHumController> _logger;
        private readonly ITemperatureRepository _temperatureRepository;
        private readonly IHumidityRepository _humidityRepository;
        private readonly IHumidityMapper _humidityMapper;
        private readonly ITemperatureMapper _temperatureMapper;


        public TempHumController(
            ILogger<TempHumController> logger,
            ITemperatureRepository temperatureRepository,
            IHumidityRepository humidityRepository,
            IHumidityMapper humidityMapper,
            ITemperatureMapper temperatureMapper
            )
        {
            _logger = logger;
            _temperatureRepository = temperatureRepository;
            _humidityRepository = humidityRepository;
            _humidityMapper = humidityMapper;
            _temperatureMapper = temperatureMapper;
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
            List<TemperatureDTO> temperatureListDTO = new List<TemperatureDTO>();

            temperatureList.ForEach(temperature =>
            {    
                var temperatureDTO = _temperatureMapper.MapToDTO(temperature);
                temperatureListDTO.Add(temperatureDTO);
                LoqRequest("GetTemperatureByDateRange", temperature.Celsius, temperature.Date);
            });
            
            return Ok(temperatureListDTO);
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
            List<HumidityDTO> humidityListDTO = new List<HumidityDTO>();

            humidityList.ForEach(humidity =>
            {
                var humidityDTO = _humidityMapper.MapToDTO(humidity);
                humidityListDTO.Add(humidityDTO);
                LoqRequest("GetTemperatureByDateRange", humidity.Percentage, humidity.Date);
            });
            
            return Ok(humidityListDTO);
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
