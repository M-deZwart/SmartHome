using Application.Application.DTOs;
using Application.Application.Interfaces;

namespace Application.Application.ApiMappers
{
    public class HumidityMapper : IHumidityMapper
    {
        public HumidityDTO MapToDTO(Humidity humidity)
        {
            return new HumidityDTO
            {
                Percentage = humidity.Percentage,
                Date = humidity.Date,
            };
        }
    }
}
