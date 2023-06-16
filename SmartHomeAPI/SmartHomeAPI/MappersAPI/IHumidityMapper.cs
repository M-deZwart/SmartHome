using SmartHomeAPI.ApplicationCore.Entities;
using SmartHomeAPI.DTOs;

namespace SmartHomeAPI.MappersAPI
{
    public interface IHumidityMapper
    {
        HumidityDTO MapToDTO(Humidity humidity);
    }
}
