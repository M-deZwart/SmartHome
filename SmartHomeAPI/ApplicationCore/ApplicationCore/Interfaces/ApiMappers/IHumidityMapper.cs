using ApplicationCore.ApplicationCore.DTOs;
using SmartHomeAPI.ApplicationCore.Entities;

namespace SmartHomeAPI.MappersAPI
{
    public interface IHumidityMapper
    {
        HumidityDTO MapToDTO(Humidity humidity);
    }
}
