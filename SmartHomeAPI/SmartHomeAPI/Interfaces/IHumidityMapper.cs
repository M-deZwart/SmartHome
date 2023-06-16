using SmartHomeAPI.ApplicationCore.Entities;
using SmartHomeAPI.DTOs;

namespace SmartHomeAPI.Interfaces
{
    public interface IHumidityMapper
    {
        HumidityDTO MapToDTO(Humidity humidity);
    }
}
