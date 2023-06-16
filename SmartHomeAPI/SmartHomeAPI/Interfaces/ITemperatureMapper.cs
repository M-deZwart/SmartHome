using SmartHomeAPI.ApplicationCore.Entities;
using SmartHomeAPI.DTOs;

namespace SmartHomeAPI.Interfaces
{
    public interface ITemperatureMapper
    {
        TemperatureDTO MapToDTO(Temperature temperature);
    }
}
