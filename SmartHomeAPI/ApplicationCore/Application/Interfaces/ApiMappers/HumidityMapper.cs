using Application.Application.DTOs;
using SmartHomeAPI.Application.Entities;
using SmartHomeAPI.MappersAPI;

namespace Application.Application.Mappers
{
    public class HumidityMapper : IHumidityMapper
    {
        public HumidityDTO MapToDTO(Humidity humidity)
        {
            return new HumidityDTO
            {
                Percentage = humidity.Percentage,
                Date = humidity.Date,
            };
        }
    }
}
