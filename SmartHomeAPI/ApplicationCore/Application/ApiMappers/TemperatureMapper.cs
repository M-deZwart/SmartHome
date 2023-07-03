using Application.Application.DTOs;
using Application.Application.Interfaces;

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
