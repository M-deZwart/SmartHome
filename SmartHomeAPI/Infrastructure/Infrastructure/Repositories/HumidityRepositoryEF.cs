using ApplicationCore.ApplicationCore.Interfaces;
using Infrastructure.Infrastructure.DTOs;
using Interfaces.Interfaces;
using SmartHomeAPI.ApplicationCore.Entities;


namespace Infrastructure.Infrastructure.Repositories
{
    public class HumidityRepositoryEF : IHumidityRepository
    {
        private readonly SmartHomeContext _smartHomeContext;
        private readonly IHumidityMapper<HumidityEfDTO> _humidityMapper;

        public HumidityRepositoryEF(SmartHomeContext smartHomeContext, IHumidityMapper<HumidityEfDTO> humidityMapper) {
            _smartHomeContext = smartHomeContext;
            _humidityMapper = humidityMapper;
        }

        public void Create(Humidity humidity)
        {
            var humidityDTO = _humidityMapper.MapToDTO(humidity);
            humidityDTO.Date.ToUniversalTime().AddHours(2);
            _smartHomeContext.Humidities.Add(humidityDTO);
            _smartHomeContext.SaveChanges();
        }

        public List<Humidity> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            var humidityListDTO = _smartHomeContext.Humidities
                .Where(h => h.Date >= startDate && h.Date <= endDate)
                .ToList();
            List<Humidity> humidityList = new List<Humidity>();

            humidityListDTO.ForEach(humidityDTO =>
            {
                var humidity = _humidityMapper.MapToEntity(humidityDTO);
                humidityList.Add(humidity);
            });

            return humidityList;
        }

        public Humidity GetById(Guid id)
        {
            var humidityDTO = _smartHomeContext.Humidities.FirstOrDefault(h => h.ID == id);

            if (humidityDTO != null)
            {
                return _humidityMapper.MapToEntity(humidityDTO);
            }
            throw new InvalidOperationException("Humidity not found");
        }
    }
}
