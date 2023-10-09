using Smarthome.Application.DTOs;
using Smarthome.Domain.Entities;

namespace Smarthome.Application.Contracts
{
    public interface ITemperatureMapper
    {
        TemperatureDTO MapToDTO(Temperature temperature);
    }
}
