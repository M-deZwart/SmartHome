using ApplicationCore.ApplicationCore.Interfaces.InfraMappers;
using Infrastructure.Infrastructure.DTOs;
using SmartHomeAPI.ApplicationCore.Entities;

namespace Infrastructure.Infrastructure.Mappers
{
    public class HumidityEfMapper : IHumidityMapper<HumidityEfDTO>
    {
        public HumidityEfDTO MapToDTO(Humidity humidity)
        {
            return new HumidityEfDTO
            {
                Percentage = humidity.Percentage,
                Date = humidity.Date,
            };
        }

        public Humidity MapToEntity(HumidityEfDTO humidityDTO)
        {
            return new Humidity
            {
                Percentage = humidityDTO.Percentage,
                Date = humidityDTO.Date
            };
        }
    }
}
