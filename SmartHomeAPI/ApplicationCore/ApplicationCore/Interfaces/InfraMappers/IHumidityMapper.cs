using SmartHomeAPI.ApplicationCore.Entities;

namespace ApplicationCore.ApplicationCore.Interfaces.InfraMappers
{
    public interface IHumidityMapper<T>
    {
        T MapToDTO(Humidity humidity);
        Humidity MapToEntity(T humidityDTO);
    }
}
