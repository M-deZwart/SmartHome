using Smarthome.Application.DTOs;
using Smarthome.Domain.Entities;

namespace Smarthome.Application.Contracts
{
    public interface IHumidityMapper
    {
        HumidityDTO MapToDTO(Humidity humidity);
    }
}
