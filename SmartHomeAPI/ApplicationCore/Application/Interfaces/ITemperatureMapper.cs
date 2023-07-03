using Application.Application.DTOs;

namespace Application.Application.Interfaces
{
    public interface ITemperatureMapper
    {
        TemperatureDTO MapToDTO(Temperature temperature);
    }
}
