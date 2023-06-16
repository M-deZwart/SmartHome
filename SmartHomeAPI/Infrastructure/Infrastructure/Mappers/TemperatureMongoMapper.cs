using Infrastructure.Infrastructure.DTOs;
using Interfaces.MappersInfra;
using SmartHomeAPI.ApplicationCore.Entities;

namespace Infrastructure.Infrastructure.Mappers
{
    internal class TemperatureMongoMapper : ITemperatureMapper<TemperatureMongoDTO>
    {
        public TemperatureMongoDTO MapToDTO(Temperature temperature)
        {
            return new TemperatureMongoDTO
            {
                Celsius = temperature.Celsius,
                Date = temperature.Date
            };
        }

        public Temperature MapToEntity(TemperatureMongoDTO temperatureDTO)
        {
            return new Temperature
            {
                Celsius = temperatureDTO.Celsius,
                Date = temperatureDTO.Date
            };
        }
    }
}
