using Infrastructure.Infrastructure.DTOs;
using Infrastructure.Infrastructure.Interfaces;
using MongoDB.Driver;
using SmartHomeAPI.ApplicationCore.Entities;
using SmartHomeAPI.ApplicationCore.Interfaces;

namespace SmartHomeAPI.Infrastructure
{
    public class HumidityRepositoryMongo : IHumidityRepository
    {
        private readonly IHumidityMapper<HumidityMongoDTO> _humidityMapper;
        private IMongoCollection<HumidityMongoDTO> _humidityCollection;

        public HumidityRepositoryMongo(IMongoDatabase db, IHumidityMapper<HumidityMongoDTO> humidityMapper)
        {
            _humidityCollection = db.GetCollection<HumidityMongoDTO>("Humidity");
            _humidityMapper = humidityMapper;
        }

        public void Create(Humidity humidity)
        {  
            var humidityDTO = _humidityMapper.MapToDTO(humidity);
            humidityDTO.Date = humidity.Date.ToUniversalTime().AddHours(2);

            _humidityCollection.InsertOne(humidityDTO);
        }

        public List<Humidity> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            var filter = Builders<HumidityMongoDTO>.Filter.And(
                    Builders<HumidityMongoDTO>.Filter.Gte(h => h.Date, startDate),
                    Builders<HumidityMongoDTO>.Filter.Lte(h => h.Date, endDate)
                );

            var humidityListDTO = _humidityCollection
                .Find(filter)
                .ToList();

            List<Humidity> humidityList = new List<Humidity>();

            humidityListDTO.ForEach(humidityDTO =>
            {
                var humidity = _humidityMapper.MapToEntity(humidityDTO);
                humidityList.Add(humidity);
            });

            return humidityList;
        }
    }
}
