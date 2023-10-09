using Smarthome.Application.DTOs;
using Application.Services;
using Smarthome.Domain.Contracts;
using Smarthome.Domain.Entities;
using Domain.Tests.Builders;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Presentation.Tests.IntegrationTests.TestInfra;
using Smarthome.Presentation.Controllers;
using Smarthome.Infrastructure.Repositories;

namespace Presentation.Tests.IntegrationTests.ControllersMongo
{
    [Collection("MongoCollection")]
    public class TemperatureControllerTests : IDisposable
    {
        // database stub setup
        private readonly IMongoDatabase _database;
        private readonly ITemperatureRepository _temperatureRepository;
        private readonly IMongoCollection<Temperature> _temperatureCollection;
        private readonly IMongoCollection<Sensor> _sensorCollection;
        private readonly MongoClient _mongoClient;
        private readonly string _databaseName;

        // controller
        private readonly TemperatureController _controller;

        // sensor
        private readonly Sensor _sensor;
        private const string SENSOR_TITLE = "LivingRoom";

        public TemperatureControllerTests(MongoFixture mongoFixture)
        {
            _mongoClient = new MongoClient(mongoFixture.ConnectionString);
            _databaseName = mongoFixture.Database;
            _database = _mongoClient.GetDatabase(_databaseName);

            _temperatureRepository = new TemperatureRepositoryMongo(_database);
            _temperatureCollection = _database.GetCollection<Temperature>("Temperature");
            _sensorCollection = _database.GetCollection<Sensor>("Sensor");

            _sensor = new Sensor(SENSOR_TITLE);
            _sensorCollection.InsertOne(_sensor);

            var temperatureService = new TemperatureService(_temperatureRepository);
            _controller = new TemperatureController(temperatureService);
        }

        [Fact]
        public async Task SetTemperature_WithValidCelsius_Should_ReturnOkAndPersistData()
        {
            // arrange
            var validCelsius = 20;

            // act
            var result = await _controller.SetTemperature(validCelsius, SENSOR_TITLE);

            // assert
            result.Should().BeOfType<OkResult>();

            var persistedTemperature = await _temperatureCollection
                .Find(x => true)
                .FirstOrDefaultAsync();
            persistedTemperature.Should().NotBeNull();
            persistedTemperature.Celsius.Should().Be(validCelsius);
        }

        [Fact]
        public async Task GetCurrentTemperature_WithValidData_ShouldReturnOkResultWithTemperatureDTO()
        {
            // Arrange
            var expectedTemperature = 20;
            var temperatureData = new TemperatureBuilder().Build();
            await _temperatureCollection.InsertOneAsync(temperatureData);

            // Act
            var result = await _controller.GetCurrentTemperature(SENSOR_TITLE);

            // Assert
            var okObjectResult =
                result.Should().BeOfType<ActionResult<TemperatureDTO>>().Subject.Result
                as OkObjectResult;
            var temperatureDto = okObjectResult?.Value as TemperatureDTO;
            temperatureDto?.Celsius.Should().Be(expectedTemperature);
        }

        [Fact]
        public async Task GetTemperatureByDateRange_WithValidRange_Should_ReturnOkResultWithTemperatureDTOList()
        {
            // arrange
            var startDate = DateTime.Now.AddHours(-24);
            var endDate = DateTime.Now;

            var mockData = new List<Temperature>
            {
                new TemperatureBuilder().WithDate(startDate.AddMinutes(30)),
                new TemperatureBuilder().WithDate(startDate.AddMinutes(90)),
                new TemperatureBuilder().WithDate(endDate.AddHours(-2))
            };

            foreach (var temperature in mockData)
            {
                await _temperatureRepository.Create(temperature, SENSOR_TITLE);
            }

            // act
            var result = await _controller.GetTemperatureByDateRange(
                startDate,
                endDate,
                SENSOR_TITLE
            );

            // assert
            var okObjectResult =
                result.Should().BeOfType<ActionResult<List<TemperatureDTO>>>().Subject.Result
                as OkObjectResult;
            var temperatureDtoList = okObjectResult?.Value as List<TemperatureDTO>;

            temperatureDtoList.Should().NotBeNull();
            temperatureDtoList.Should().HaveCount(3);

            temperatureDtoList
                ?[0].Date.Should()
                .BeCloseTo(
                    mockData.ElementAt(0).Date.ToUniversalTime(),
                    precision: TimeSpan.FromSeconds(1)
                );
            temperatureDtoList
                ?[1].Date.Should()
                .BeCloseTo(
                    mockData.ElementAt(1).Date.ToUniversalTime(),
                    precision: TimeSpan.FromSeconds(1)
                );
            temperatureDtoList
                ?[2].Date.Should()
                .BeCloseTo(
                    mockData.ElementAt(2).Date.ToUniversalTime(),
                    precision: TimeSpan.FromSeconds(1)
                );
        }

        [Fact]
        public async Task SetTemperature_WithInvalidCelsius_Should_ReturnBadRequestWithValidationErrors()
        {
            // arrange
            var invalidCelsius = -10;

            // act
            var result = await _controller.SetTemperature(invalidCelsius, SENSOR_TITLE);

            // assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            var errorMessage = badRequestResult?.Value as string;
            errorMessage.Should().Contain("Validation errors occurred");
        }

        public void Dispose()
        {
            _database.DropCollection("Temperature");
        }
    }
}
