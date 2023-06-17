using SmartHomeAPI.ApplicationCore.Entities;

namespace ApplicationCore.ApplicationCore.Interfaces.InfraMappers
{
    public interface ITemperatureMapper<T>
    {
        T MapToDTO(Temperature temperature);
        Temperature MapToEntity(T temperatureDTO);
    }
}
