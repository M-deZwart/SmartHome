using Smarthome.Application.ApiMappers;
using Smarthome.Application.Contracts;
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
    public class HumidityControllerTests : IDisposable
    {
        // database stub setup
        private readonly IMongoDatabase _database;
        private readonly IHumidityRepository _humidityRepository;
        private readonly IMongoCollection<Humidity> _humidityCollection;
        private readonly IMongoCollection<Sensor> _sensorCollection;
        private readonly MongoClient _mongoClient;
        private readonly string _databaseName;

        // mapper
        private readonly IHumidityMapper _mapper;

        // controller
        private readonly HumidityController _controller;

        // sensor
        private readonly Sensor _sensor;
        private const string SENSOR_TITLE = "LivingRoom";

        public HumidityControllerTests(MongoFixture mongoFixture)
        {
            _mongoClient = new MongoClient(mongoFixture.ConnectionString);
            _databaseName = mongoFixture.Database;
            _database = _mongoClient.GetDatabase(_databaseName);

            _humidityRepository = new HumidityRepositoryMongo(_database);
            _humidityCollection = _database.GetCollection<Humidity>("Humidity");
            _sensorCollection = _database.GetCollection<Sensor>("Sensor");

            _sensor = new Sensor(SENSOR_TITLE);
            _sensorCollection.InsertOne(_sensor);

            _mapper = new HumidityMapper();

            var humidityService = new HumidityService(_humidityRepository, _mapper);
            _controller = new HumidityController(humidityService);
        }

        [Fact]
        public async Task SetHumidity_WithValidPercentage_Should_ReturnOkAndPersistData()
        {
            // arrange
            double validPercentage = 50;

            // act
            var result = await _controller.SetHumidity(validPercentage, SENSOR_TITLE);

            // assert
            result.Should().BeOfType<OkResult>();

            var persistedHumidity = await _humidityCollection.Find(x => true).FirstOrDefaultAsync();
            persistedHumidity.Should().NotBeNull();
            persistedHumidity.Percentage.Should().Be(validPercentage);
        }

        [Fact]
        public async Task GetCurrentHumidity_WithValidData_ShouldReturnOkResultWithHumidityDTO()
        {
            // arrange
            var expectedPercentage = 50;
            var humidityData = new HumidityBuilder().Build();
            await _humidityCollection.InsertOneAsync(humidityData);

            // act
            var result = await _controller.GetCurrentHumidity(SENSOR_TITLE);

            // assert
            var okObjectResult =
                result.Should().BeOfType<ActionResult<HumidityDTO>>().Subject.Result
                as OkObjectResult;
            var humidityDto = okObjectResult?.Value as HumidityDTO;
            humidityDto?.Percentage.Should().Be(expectedPercentage);
        }

        [Fact]
        public async Task GetHumidityByDateRange_WithValidRange_Should_ReturnOkResultWithHumidityDTOList()
        {
            // arrange
            var startDate = DateTime.Now.AddHours(-24);
            var endDate = DateTime.Now;

            var mockData = new List<Humidity>
            {
                new HumidityBuilder().WithDate(startDate.AddMinutes(30)),
                new HumidityBuilder().WithDate(startDate.AddMinutes(90)),
                new HumidityBuilder().WithDate(endDate.AddHours(-2))
            };

            foreach (var humidity in mockData)
            {
                await _humidityRepository.Create(humidity, SENSOR_TITLE);
            }

            // act
            var result = await _controller.GetHumidityByDateRange(startDate, endDate, SENSOR_TITLE);

            // assert
            result.Should().BeOfType<ActionResult<List<HumidityDTO>>>();
            var okObjectResult =
                result.Should().BeOfType<ActionResult<List<HumidityDTO>>>().Subject.Result
                as OkObjectResult;
            var humidityDtoList = okObjectResult?.Value as List<HumidityDTO>;

            humidityDtoList.Should().NotBeNull();
            humidityDtoList.Should().HaveCount(3);

            humidityDtoList
                ?[0].Date.Should()
                .BeCloseTo(
                    mockData.ElementAt(0).Date.ToUniversalTime(),
                    precision: TimeSpan.FromSeconds(1)
                );
            humidityDtoList
                ?[1].Date.Should()
                .BeCloseTo(
                    mockData.ElementAt(1).Date.ToUniversalTime(),
                    precision: TimeSpan.FromSeconds(1)
                );
            humidityDtoList
                ?[2].Date.Should()
                .BeCloseTo(
                    mockData.ElementAt(2).Date.ToUniversalTime(),
                    precision: TimeSpan.FromSeconds(1)
                );
        }

        [Fact]
        public async Task SetHumidity_WithInvalidPercentage_Should_ReturnBadRequestWithValidationErrors()
        {
            // arrange
            double invalidPercentage = -10;

            // act
            var result = await _controller.SetHumidity(invalidPercentage, SENSOR_TITLE);

            // assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            var errorMessage = badRequestResult?.Value as string;
            errorMessage.Should().Contain("Validation errors occurred");
        }

        public void Dispose()
        {
            _database.DropCollection("Humidity");
        }
    }
}
