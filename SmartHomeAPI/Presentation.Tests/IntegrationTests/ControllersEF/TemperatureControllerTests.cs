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
    public class TemperatureControllerTests
    {
        private readonly SmartHomeContext _context;
        private readonly TemperatureRepositoryEF _temperatureRepository;
        private readonly ITemperatureMapper _mapper;
        private readonly TemperatureController _controller;
        private const string SENSOR_TITLE = "LivingRoom";

        public TemperatureControllerTests()
        {
            _context = CreateTestContext();
            _temperatureRepository = new TemperatureRepositoryEF(_context);

            _mapper = new TemperatureMapper();
            var temperatureService = new TemperatureService(_temperatureRepository, _mapper);
            _controller = new TemperatureController(temperatureService);
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
        public async Task SetTemperature_WithValidCelsius_Should_ReturnOkAndPersistData()
        {
            // arrange
            double validCelsius = 20;

            // act
            var result = await _controller.SetTemperature(validCelsius, SENSOR_TITLE);

            // assert
            result.Should().BeOfType<OkResult>();

            var persistedTemperature = _context.Temperatures.FirstOrDefault();
            persistedTemperature.Should().NotBeNull();
            persistedTemperature?.Celsius.Should().Be(validCelsius);
        }

        [Fact]
        public async Task GetCurrentTemperature_WithValidData_ShouldReturnOkResultWithTemperatureDTO()
        {
            // arrange
            var expectedCelsius = 20;
            var temperatureData = new TemperatureBuilder().Build();
            _context.Temperatures.Add(temperatureData);

            // act
            var result = await _controller.GetCurrentTemperature(SENSOR_TITLE);

            // assert
            var okObjectResult = result.Should().BeOfType<ActionResult<TemperatureDTO>>().Subject.Result as OkObjectResult;
            var temperatureDto = okObjectResult?.Value as TemperatureDTO;
            temperatureDto?.Celsius.Should().Be(expectedCelsius);
        }

        [Fact]
        public async Task GetTemperatureByDateRange_WithValidRange_Should_ReturnOkResultWithTemperatureDTOList()
        {
            // arrange
            var startDate = DateTime.Now.AddHours(-24);
            var endDate = DateTime.Now;
            Temperature temperatureData1 = new TemperatureBuilder().WithDate(startDate.AddMinutes(30));
            Temperature temperatureData2 = new TemperatureBuilder().WithDate(startDate.AddMinutes(90));
            Temperature temperatureData3 = new TemperatureBuilder().WithDate(endDate.AddHours(-2));

            await _context.Temperatures.AddRangeAsync(new List<Temperature> { temperatureData1, temperatureData2, temperatureData3 });
            await _context.SaveChangesAsync();

            // act
            var result = await _controller.GetTemperatureByDateRange(startDate, endDate, SENSOR_TITLE);

            // assert
            result.Should().BeOfType<ActionResult<List<TemperatureDTO>>>();
            var okObjectResult = result.Should().BeOfType<ActionResult<List<TemperatureDTO>>>().Subject.Result as OkObjectResult;
            var temperatureDtoList = okObjectResult?.Value as List<TemperatureDTO>;

            temperatureDtoList.Should().NotBeNull();
            temperatureDtoList.Should().HaveCount(3);

            temperatureDtoList?[0].Date.Should().BeCloseTo(temperatureData1.Date, precision: TimeSpan.FromSeconds(1));
            temperatureDtoList?[1].Date.Should().BeCloseTo(temperatureData2.Date, precision: TimeSpan.FromSeconds(1));
            temperatureDtoList?[2].Date.Should().BeCloseTo(temperatureData3.Date, precision: TimeSpan.FromSeconds(1));
        }
    }
}
