using Smarthome.Presentation.Controllers;
using Application.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Smarthome.Application.DTOs;
using Domain.Tests.Builders;
using Smarthome.Domain.Entities;
using Smarthome.Infrastructure.Repositories;

namespace Presentation.Tests.IntegrationTests.ControllersEF
{
    public class TemperatureControllerTests : CommonTestBase
    {
        private readonly TemperatureRepositoryEF _temperatureRepository;
        private readonly TemperatureController _controller;

        public TemperatureControllerTests()
        {
            _temperatureRepository = new TemperatureRepositoryEF(Context);
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

            var persistedTemperature = Context.Temperatures.FirstOrDefault();
            persistedTemperature.Should().NotBeNull();
            persistedTemperature?.Celsius.Should().Be(validCelsius);
        }

        [Fact]
        public async Task GetCurrentTemperature_WithValidData_ShouldReturnOkResultWithTemperatureDTO()
        {
            // arrange
            var expectedCelsius = 20;
            var temperatureData = new TemperatureBuilder().Build();
            Context.Temperatures.Add(temperatureData);

            // act
            var result = await _controller.GetCurrentTemperature(SENSOR_TITLE);

            // assert
            var okObjectResult =
                result.Should().BeOfType<ActionResult<TemperatureDTO>>().Subject.Result
                as OkObjectResult;
            var temperatureDto = okObjectResult?.Value as TemperatureDTO;
            temperatureDto?.Celsius.Should().Be(expectedCelsius);
        }

        [Fact]
        public async Task GetTemperatureByDateRange_WithValidRange_Should_ReturnOkResultWithTemperatureDTOList()
        {
            // arrange
            var startDate = DateTime.Now.AddHours(-24);
            var endDate = DateTime.Now;
            var mockData = new List<Temperature>()
            {
                new TemperatureBuilder().WithDate(startDate.AddMinutes(30)),
                new TemperatureBuilder().WithDate(startDate.AddMinutes(90)),
                new TemperatureBuilder().WithDate(endDate.AddHours(-2)),
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
            result.Should().BeOfType<ActionResult<List<TemperatureDTO>>>();
            var okObjectResult =
                result.Should().BeOfType<ActionResult<List<TemperatureDTO>>>().Subject.Result
                as OkObjectResult;
            var temperatureDtoList = okObjectResult?.Value as List<TemperatureDTO>;

            temperatureDtoList.Should().NotBeNull();
            temperatureDtoList.Should().HaveCount(3);

            temperatureDtoList
                ?[0].Date.Should()
                .BeCloseTo(mockData.ElementAt(0).Date, precision: TimeSpan.FromSeconds(1));
            temperatureDtoList
                ?[1].Date.Should()
                .BeCloseTo(mockData.ElementAt(1).Date, precision: TimeSpan.FromSeconds(1));
            temperatureDtoList
                ?[2].Date.Should()
                .BeCloseTo(mockData.ElementAt(2).Date, precision: TimeSpan.FromSeconds(1));
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
    }
}
