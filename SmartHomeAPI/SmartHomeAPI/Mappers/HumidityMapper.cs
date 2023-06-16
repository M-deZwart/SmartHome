using SmartHomeAPI.ApplicationCore.Entities;
using SmartHomeAPI.DTOs;
using SmartHomeAPI.MappersAPI;

namespace SmartHomeAPI.Mappers
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
