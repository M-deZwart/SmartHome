using ApplicationCore.ApplicationCore.Interfaces;
using Infrastructure.Infrastructure.DTOs;
using SmartHomeAPI.ApplicationCore.Entities;

namespace Infrastructure.Infrastructure.Mappers
{
    public class HumidityMongoMapper : IHumidityMapper<HumidityMongoDTO>
    {
        public HumidityMongoDTO MapToDTO(Humidity humidity)
        {
            return new HumidityMongoDTO
            {
                Percentage = humidity.Percentage,
                Date = humidity.Date
            };
        }

        public Humidity MapToEntity(HumidityMongoDTO humidityDTO)
        {
            return new Humidity
            {
                Percentage = humidityDTO.Percentage,
                Date = humidityDTO.Date
            };
        }
    }
}
