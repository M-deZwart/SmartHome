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
    public class HumidityControllerTests : IDisposable
    {
        // database stub setup
        private readonly IMongoDatabase _database;
        private readonly IHumidityRepository _humidityRepository;
        private readonly IMongoCollection<Humidity> _humidityCollection;
        private readonly MongoClient _mongoClient;
        private readonly string _databaseName;
        // mapper
        private readonly IHumidityMapper _mapper;
        // controller
        private readonly HumidityController _controller;

        public HumidityControllerTests(MongoFixture mongoFixture)
        {
            _mongoClient = new MongoClient(mongoFixture.ConnectionString);
            _databaseName = mongoFixture.Database;
            _database = _mongoClient.GetDatabase(_databaseName);

            _humidityRepository = new HumidityRepositoryMongo(_database);
            _humidityCollection = _database.GetCollection<Humidity>("Humidity");

            _mapper = new HumidityMapper();

            var humidityService = new HumidityService(_humidityRepository, _mapper);
            _controller = new HumidityController(humidityService);
        }

        [Fact]
        public async Task SetHumidity_WithValidPercentage_Should_ReturnOkAndPersistData()
        {
            // Arrange
            double validPercentage = 50;

            // Act
            var result = await _controller.SetHumidity(validPercentage);

            // Assert
            result.Should().BeOfType<OkResult>();

            var persistedHumidity = await _humidityCollection.Find(x => true).FirstOrDefaultAsync();
            persistedHumidity.Should().NotBeNull();
            persistedHumidity.Percentage.Should().Be(validPercentage);
        }

        [Fact]
        public async Task GetCurrentHumidity_WithValidData_ShouldReturnOkResultWithHumidityDTO()
        {
            // Arrange
            var humidityData = new HumidityBuilder().Build();
           
            await _humidityCollection.InsertOneAsync(humidityData);

            // Act
            var result = await _controller.GetCurrentHumidity();

            // Assert
            result.Should().BeOfType<ActionResult<HumidityDTO>>();
        }

        [Fact]
        public async Task GetHumidityByDateRange_WithValidRange_Should_ReturnOkResultWithHumidityDTOList()
        {
            // Arrange
            var startDate = DateTime.Now.AddHours(-24);
            var endDate = DateTime.Now;
            var humidityData1 = new HumidityBuilder().WithDate(startDate.AddMinutes(30));
            var humidityData2 = new HumidityBuilder().WithDate(startDate.AddMinutes(90));
            var humidityData3 = new HumidityBuilder().WithDate(endDate.AddHours(-2));
            await _humidityCollection.InsertManyAsync(new List<Humidity> { humidityData1, humidityData2, humidityData3 });

            // Act
            var result = await _controller.GetHumidityByDateRange(startDate, endDate);

            // Assert
            result.Should().BeOfType<ActionResult<List<HumidityDTO>>>();
        }

        public void Dispose()
        {
            _database.DropCollection("Humidity");
        }
    }
}
