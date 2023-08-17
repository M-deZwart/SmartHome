using Application.Application.ApiMappers;
using Application.Application.Contracts;
using Application.Application.DTOs;
using Application.Application.Services;
using Domain.Domain.Contracts;
using Domain.Domain.Entities;
using Domain.Tests.Builders;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Presentation.Tests.IntegrationTests.TestInfra;
using SmartHomeAPI.Controllers;
using SmartHomeAPI.Infrastructure.Repositories;

namespace Presentation.Tests.IntegrationTests.ControllersMongo
{
    [Collection("MongoCollection")]
    public class TemperatureControllerTests : IDisposable
    {
        // database stub setup
        private readonly IMongoDatabase _database;
        private readonly ITemperatureRepository _temperatureRepository;
        private readonly IMongoCollection<Temperature> _temperatureCollection;
        private readonly MongoClient _mongoClient;
        private readonly string _databaseName;
        // mapper
        private readonly ITemperatureMapper _mapper;
        // controller
        private readonly TemperatureController _controller;

        public TemperatureControllerTests(MongoFixture mongoFixture)
        {
            _mongoClient = new MongoClient(mongoFixture.ConnectionString);
            _databaseName = mongoFixture.Database;
            _database = _mongoClient.GetDatabase(_databaseName);

            _temperatureRepository = new TemperatureRepositoryMongo(_database);
            _temperatureCollection = _database.GetCollection<Temperature>("Temperature");

            _mapper = new TemperatureMapper();

            var temperatureService = new TemperatureService(_temperatureRepository, _mapper);
            _controller = new TemperatureController(temperatureService);
        }

        [Fact]
        public async Task SetTemperature_WithValidCelsius_Should_ReturnOkAndPersistData()
        {
            // Arrange
            double validCelsius = 20;

            // Act
            var result = await _controller.SetTemperature(validCelsius);

            // Assert
            result.Should().BeOfType<OkResult>();

            var persistedTemperature = await _temperatureCollection.Find(x => true).FirstOrDefaultAsync();
            persistedTemperature.Should().NotBeNull();
            persistedTemperature.Celsius.Should().Be(validCelsius);
        }

        [Fact]
        public async Task GetCurrentTemperature_WithValidData_ShouldReturnOkResultWithTemperatureDTO()
        {
            // Arrange
            var temperatureData = new TemperatureBuilder().Build();

            await _temperatureCollection.InsertOneAsync(temperatureData);

            // Act
            var result = await _controller.GetCurrentTemperature();

            // Assert
            result.Should().BeOfType<ActionResult<TemperatureDTO>>();
        }

        [Fact]
        public async Task GetTemperatureByDateRange_WithValidRange_Should_ReturnOkResultWithTemperatureDTOList()
        {
            // Arrange
            var startDate = DateTime.Now.AddHours(-24);
            var endDate = DateTime.Now;
            var temperatureData1 = new TemperatureBuilder().WithDate(startDate.AddMinutes(30));
            var temperatureData2 = new TemperatureBuilder().WithDate(startDate.AddMinutes(90));
            var temperatureData3 = new TemperatureBuilder().WithDate(endDate.AddHours(-2));
            await _temperatureCollection.InsertManyAsync(new List<Temperature> { temperatureData1, temperatureData2, temperatureData3 });

            // Act
            var result = await _controller.GetTemperatureByDateRange(startDate, endDate);

            // Assert
            result.Should().BeOfType<ActionResult<List<TemperatureDTO>>>();
        }

        public void Dispose()
        {
            _database.DropCollection("Humidity");
        }
    }
}
