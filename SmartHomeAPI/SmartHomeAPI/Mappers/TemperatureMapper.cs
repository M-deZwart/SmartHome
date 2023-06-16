using SmartHomeAPI.ApplicationCore.Entities;
using SmartHomeAPI.DTOs;
using SmartHomeAPI.Interfaces;

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
