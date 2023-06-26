﻿using ApplicationCore.ApplicationCore.Interfaces.InfraMappers;
using Infrastructure.Infrastructure.DTOs;
using Interfaces.Interfaces;
using MongoDB.Driver;
using SmartHomeAPI.ApplicationCore.Entities;

namespace SmartHomeAPI.Infrastructure.Repositories
{
    public class HumidityRepositoryMongo : IHumidityRepository
    {
        private readonly IHumidityMapper<HumidityMongoDTO> _humidityMapper;
        private readonly IMongoCollection<HumidityMongoDTO> _humidityCollection;

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

        public Humidity GetById(Guid id)
        {
            var filter = Builders<HumidityMongoDTO>.Filter.Eq(h => h.ID, id);
            var humidityDTO = _humidityCollection.Find(filter).FirstOrDefault();

            if (humidityDTO is not null)
            {
                return _humidityMapper.MapToEntity(humidityDTO) ;
            }

            throw new InvalidOperationException("Humidity not found");
        }
    }
}
