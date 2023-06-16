using Infrastructure.Infrastructure.DTOs;
using Infrastructure.Infrastructure.Interfaces;
using SmartHomeAPI.ApplicationCore.Entities;

namespace Infrastructure.Infrastructure.Mappers
{
    public class TemperatureEfMapper : ITemperatureMapper<TemperatureEfDTO>
    {
        public TemperatureEfDTO MapToDTO(Temperature temperature)
        {
            return new TemperatureEfDTO
            {
                Celsius = temperature.Celsius,
                Date = temperature.Date
            };
        }

        public Temperature MapToEntity(TemperatureEfDTO temperatureDTO)
        {
            return new Temperature
            {
                Celsius = temperatureDTO.Celsius,
                Date = temperatureDTO.Date
            };
        }
    }
}
