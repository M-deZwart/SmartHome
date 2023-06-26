﻿using Infrastructure.Infrastructure.DTOs;
using Interfaces.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.ApplicationCore.Entities;
using SmartHomeAPI.DTOs;
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
        public IActionResult SetHumidity([FromRoute] float percentage)
        {
            Humidity humidity;

            humidity = new Humidity
            {
                Percentage = percentage,
                Date = DateTime.Now
            };

            _humidityRepository.Create(humidity);

            _requestLogger.LogRequest("SetHumidity", humidity.Percentage, humidity.Date);
            return Ok(humidity);
        }

        [HttpGet("{id}")]
        public IActionResult GetCurrentHumidity(Guid id)
        {
            try
            {
                var humidity = _humidityRepository.GetById(id);

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
                return StatusCode(500, ex.Message);
            }
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
                _requestLogger.LogRequest("GetTemperatureByDateRange", humidityDTO.Percentage, humidityDTO.Date);
            });

            return Ok(humidityListDTO);
        }

    }
}