using Smarthome.Application.DTOs;
using Smarthome.Application.Contracts;
using Smarthome.Domain.Entities;

namespace Smarthome.Application.ApiMappers
{
    public class HumidityMapper : IHumidityMapper
    {
        public HumidityDTO MapToDTO(Humidity humidity)
        {
            return new HumidityDTO(Percentage: humidity.Percentage, Date: humidity.Date);
        }
    }
}
