using SmartHomeAPI.ApplicationCore.Entities;

namespace Infrastructure.Infrastructure.Interfaces
{
    public interface IHumidityMapper<T>
    {
        T MapToDTO(Humidity humidity);
        Humidity MapToEntity(T humidityDTO);
    }
}
