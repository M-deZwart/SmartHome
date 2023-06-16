using Infrastructure.Infrastructure.DTOs;
using Infrastructure.Infrastructure.Interfaces;
using SmartHomeAPI.ApplicationCore.Entities;
using SmartHomeAPI.ApplicationCore.Interfaces;

namespace Infrastructure.Infrastructure
{
    public class TemperatureRepositoryEF : ITemperatureRepository
    {
        private readonly SmartHomeContext _smartHomeContext;
        private readonly ITemperatureMapper<TemperatureEfDTO> _temperatureMapper;

        public TemperatureRepositoryEF(SmartHomeContext smartHomeContext, ITemperatureMapper<TemperatureEfDTO> temperatureMapper)
        {
            _smartHomeContext = smartHomeContext;
            _temperatureMapper = temperatureMapper;
        }

        public void Create(Temperature temperature)
        {
            var temperatureDTO = _temperatureMapper.MapToDTO(temperature);
            temperatureDTO.Date = temperature.Date.ToUniversalTime().AddHours(2);
            _smartHomeContext.Temperatures.Add(temperatureDTO);
            _smartHomeContext.SaveChanges();
        }

        public List<Temperature> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            var temperatureListDTO = _smartHomeContext.Temperatures
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .ToList();
            List<Temperature> temperatureList = new List<Temperature>();

            temperatureListDTO.ForEach(temperatureDTO =>
            {
                var temperature = _temperatureMapper.MapToEntity(temperatureDTO);
                temperatureList.Add(temperature);
            });

            return temperatureList;
        }
    }
}
