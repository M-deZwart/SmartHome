using Application.Application.DTOs;
using SmartHomeAPI.Application.Entities;
using SmartHomeAPI.MappersAPI;

namespace SmartHomeAPI.Mappers
{
    public class TemperatureMapper :ITemperatureMapper
    {
        public TemperatureDTO MapToDTO(Temperature temperature)
        {
            return new TemperatureDTO
            {
                Celsius = temperature.Celsius,
                Date = temperature.Date
            };
        }
    }
}
