using SmartHomeAPI.ApplicationCore.Entities;

namespace Interfaces.MappersInfra
{
    public interface IHumidityMapper<T>
    {
        T MapToDTO(Humidity humidity);
        Humidity MapToEntity(T humidityDTO);
    }
}
