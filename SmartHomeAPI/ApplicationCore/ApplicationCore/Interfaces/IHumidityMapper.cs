using SmartHomeAPI.ApplicationCore.Entities;

namespace ApplicationCore.ApplicationCore.Interfaces
{
    public interface IHumidityMapper<T>
    {
        T MapToDTO(Humidity humidity);
        Humidity MapToEntity(T humidityDTO);
    }
}
