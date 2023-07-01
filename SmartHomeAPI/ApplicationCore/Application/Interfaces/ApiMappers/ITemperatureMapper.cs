using Application.Application.DTOs;
using SmartHomeAPI.Application.Entities;

namespace SmartHomeAPI.MappersAPI
{
    public interface ITemperatureMapper
    {
        TemperatureDTO MapToDTO(Temperature temperature);
    }
}
