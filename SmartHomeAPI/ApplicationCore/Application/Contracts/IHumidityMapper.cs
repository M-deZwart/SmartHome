using Application.Application.DTOs;
using Domain.Domain.Entities;

namespace Application.Application.Contracts
{
    public interface IHumidityMapper
    {
        HumidityDTO MapToDTO(Humidity humidity);
    }
}
