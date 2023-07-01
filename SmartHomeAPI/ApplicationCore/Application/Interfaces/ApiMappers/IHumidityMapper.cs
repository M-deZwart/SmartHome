using Application.Application.DTOs;
using SmartHomeAPI.Application.Entities;

namespace SmartHomeAPI.MappersAPI
{
    public interface IHumidityMapper
    {
        HumidityDTO MapToDTO(Humidity humidity);
    }
}
