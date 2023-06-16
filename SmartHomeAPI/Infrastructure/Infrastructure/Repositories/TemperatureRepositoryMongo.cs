using Infrastructure.Infrastructure.DTOs;
using Infrastructure.Infrastructure.Interfaces;
using MongoDB.Driver;
using SmartHomeAPI.ApplicationCore.Entities;
using SmartHomeAPI.ApplicationCore.Interfaces;

namespace SmartHomeAPI.Infrastructure
{
    public class TemperatureRepositoryMongo : ITemperatureRepository
    {
        private readonly ITemperatureMapper<TemperatureMongoDTO> _temperatureMapper;
        private IMongoCollection<TemperatureMongoDTO> _temperatureCollection;

        public TemperatureRepositoryMongo(IMongoDatabase db, ITemperatureMapper<TemperatureMongoDTO> temperatureMapper)
        {
            _temperatureCollection = db.GetCollection<TemperatureMongoDTO>("Temperature");
            _temperatureMapper = temperatureMapper;
        }

        public void Create(Temperature temperature)
        {
            var temperatureDTO = _temperatureMapper.MapToDTO(temperature);
            temperature.Date = temperature.Date.ToUniversalTime().AddHours(2);
            _temperatureCollection.InsertOne(temperatureDTO);
        }

        public List<Temperature> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            var filter = Builders<TemperatureMongoDTO>.Filter.And(
                    Builders<TemperatureMongoDTO>.Filter.Gte(t => t.Date, startDate),
                    Builders<TemperatureMongoDTO>.Filter.Lte(t => t.Date, endDate)
                );

            var temperatureListDTO = _temperatureCollection
                .Find(filter)
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
