using Application.Application.DTOs;
using Application.Application.Interfaces;
using Domain.Domain.Entities;

namespace Application.Application.ApiMappers
{
    public class TemperatureMapper : ITemperatureMapper
    {
        public TemperatureDTO MapToDTO(Temperature temperature)
        {
            return new TemperatureDTO
            {
                Celsius = temperature.Celsius,
                Date = temperature.Date
            };
        }
    }
}
