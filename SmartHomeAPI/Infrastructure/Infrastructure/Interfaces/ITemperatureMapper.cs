using SmartHomeAPI.ApplicationCore.Entities;

namespace Infrastructure.Infrastructure.Interfaces
{
    public interface ITemperatureMapper<T>
    {
        T MapToDTO(Temperature temperature);
        Temperature MapToEntity(T temperatureDTO);
    }
}
