using Application.Application.DTOs;

namespace Application.Application.Interfaces
{
    public interface IHumidityMapper
    {
        HumidityDTO MapToDTO(Humidity humidity);
    }
}
