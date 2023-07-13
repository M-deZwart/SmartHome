using Application.Application.DTOs;
using Domain.Domain.Entities;

namespace Application.Application.Interfaces
{
    public interface IHumidityMapper
    {
        HumidityDTO MapToDTO(Humidity humidity);
    }
}
