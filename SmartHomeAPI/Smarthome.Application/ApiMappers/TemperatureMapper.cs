using Smarthome.Application.DTOs;
using Smarthome.Application.Contracts;
using Smarthome.Domain.Entities;

namespace Smarthome.Application.ApiMappers
{
    public class TemperatureMapper : ITemperatureMapper
    {
        public TemperatureDTO MapToDTO(Temperature temperature)
        {
            return new TemperatureDTO(Celsius: temperature.Celsius, Date: temperature.Date);
        }
    }
}
