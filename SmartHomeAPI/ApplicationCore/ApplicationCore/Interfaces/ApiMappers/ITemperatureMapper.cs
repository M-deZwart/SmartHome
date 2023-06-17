using SmartHomeAPI.ApplicationCore.Entities;
using SmartHomeAPI.DTOs;

namespace SmartHomeAPI.MappersAPI
{
    public interface ITemperatureMapper
    {
        TemperatureDTO MapToDTO(Temperature temperature);
    }
}
