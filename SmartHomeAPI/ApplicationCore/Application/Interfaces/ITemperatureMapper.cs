using Application.Application.DTOs;
using Domain.Domain.Entities;

namespace Application.Application.Interfaces
{
    public interface ITemperatureMapper
    {
        TemperatureDTO MapToDTO(Temperature temperature);
    }
}
