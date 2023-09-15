using Infrastructure.Infrastructure.Repositories;
using Infrastructure.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Application.Application.Contracts;
using SmartHomeAPI.Controllers;
using Application.Application.ApiMappers;
using Application.Application.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Domain.Domain.Entities;
using Application.Application.DTOs;
using Domain.Tests.Builders;

namespace Presentation.Tests.IntegrationTests.ControllersEF
{
    public class HumidityControllerTests
    {
        private readonly SmartHomeContext _context;
        private readonly HumidityRepositoryEF _humidityRepository;
        private readonly IHumidityMapper _mapper;
        private readonly HumidityController _controller;
        private readonly Sensor _sensor;
        private const string SENSOR_TITLE = "LivingRoom";

        public HumidityControllerTests()
        {
            _context = CreateTestContext();
            _humidityRepository = new HumidityRepositoryEF(_context);

            _mapper = new HumidityMapper();
            var humidityService = new HumidityService(_humidityRepository, _mapper);
            _controller = new HumidityController(humidityService);

            _sensor = new Sensor(SENSOR_TITLE);
            _context.Sensors.Add(_sensor);
            _context.SaveChanges();
        }

        private DbContextOptions<SmartHomeContext> CreateNewInMemoryDatabase()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SmartHomeContext>();
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            return optionsBuilder.Options;
        }

        private SmartHomeContext CreateTestContext()
        {
            var options = CreateNewInMemoryDatabase();
            var context = new SmartHomeContext(options);
            return context;
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

            var persistedHumidity = _context.Humidities.FirstOrDefault();
            persistedHumidity.Should().NotBeNull();
            persistedHumidity?.Percentage.Should().Be(validPercentage);
        }

        [Fact]
        public async Task GetCurrentHumidity_WithValidData_ShouldReturnOkResultWithHumidityDTO()
        {
            // arrange
            var expectedPercentage = 50;
            var humidityData = new HumidityBuilder().Build();
            _context.Humidities.Add(humidityData);

            // act
            var result = await _controller.GetCurrentHumidity(SENSOR_TITLE);

            // assert
            var okObjectResult = result.Should().BeOfType<ActionResult<HumidityDTO>>().Subject.Result as OkObjectResult;
            var humidityDto = okObjectResult?.Value as HumidityDTO;
            humidityDto?.Percentage.Should().Be(expectedPercentage);
        }

        [Fact]
        public async Task GetHumidityByDateRange_WithValidRange_Should_ReturnOkResultWithHumidityDTOList()
        {
            // arrange
            var startDate = DateTime.Now.AddHours(-24);
            var endDate = DateTime.Now;
            Humidity humidityData1 = new HumidityBuilder().WithDate(startDate.AddMinutes(30));
            Humidity humidityData2 = new HumidityBuilder().WithDate(startDate.AddMinutes(90));
            Humidity humidityData3 = new HumidityBuilder().WithDate(endDate.AddHours(-2));

            await _context.Humidities.AddRangeAsync(new List<Humidity> { humidityData1, humidityData2, humidityData3 });
            await _context.SaveChangesAsync();

            // act
            var result = await _controller.GetHumidityByDateRange(startDate, endDate, SENSOR_TITLE);

            // assert
            result.Should().BeOfType<ActionResult<List<HumidityDTO>>>();
            var okObjectResult = result.Should().BeOfType<ActionResult<List<HumidityDTO>>>().Subject.Result as OkObjectResult;
            var humidityDtoList = okObjectResult?.Value as List<HumidityDTO>;

            humidityDtoList.Should().NotBeNull();
            humidityDtoList.Should().HaveCount(3);

            humidityDtoList?[0].Date.Should().BeCloseTo(humidityData1.Date, precision: TimeSpan.FromSeconds(1));
            humidityDtoList?[1].Date.Should().BeCloseTo(humidityData2.Date, precision: TimeSpan.FromSeconds(1));
            humidityDtoList?[2].Date.Should().BeCloseTo(humidityData3.Date, precision: TimeSpan.FromSeconds(1));
        }
    }
}
