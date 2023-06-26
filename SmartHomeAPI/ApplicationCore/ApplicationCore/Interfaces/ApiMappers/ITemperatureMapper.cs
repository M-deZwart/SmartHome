using ApplicationCore.ApplicationCore.DTOs;
using SmartHomeAPI.ApplicationCore.Entities;

namespace SmartHomeAPI.MappersAPI
{
    public interface ITemperatureMapper
    {
        TemperatureDTO MapToDTO(Temperature temperature);
    }
}
