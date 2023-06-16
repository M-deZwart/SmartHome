using SmartHomeAPI.ApplicationCore.Entities;

namespace Interfaces.MappersInfra
{
    public interface ITemperatureMapper<T>
    {
        T MapToDTO(Temperature temperature);
        Temperature MapToEntity(T temperatureDTO);
    }
}
