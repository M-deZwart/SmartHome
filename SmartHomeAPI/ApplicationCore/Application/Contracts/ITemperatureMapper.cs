using Application.Application.DTOs;
using Domain.Domain.Entities;

namespace Application.Application.Contracts
{
    public interface ITemperatureMapper
    {
        TemperatureDTO MapToDTO(Temperature temperature);
    }
}
