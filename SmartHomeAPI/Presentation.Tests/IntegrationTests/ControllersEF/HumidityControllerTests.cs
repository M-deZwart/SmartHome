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
    public class HumidityControllerTests : CommonTestBase
    {
        private readonly HumidityRepositoryEF _humidityRepository;
        private readonly HumidityController _controller;

        public HumidityControllerTests()
        {
            _humidityRepository = new HumidityRepositoryEF(Context);
            var humidityService = new HumidityService(_humidityRepository);
            _controller = new HumidityController(humidityService);
        }

        [Fact]
        public async Task SetHumidity_WithValidPercentage_Should_ReturnOkAndPersistData()
        {
            // arrange
            var validPercentage = 50;

            // act
            var result = await _controller.SetHumidity(validPercentage, SENSOR_TITLE);

            // assert
            result.Should().BeOfType<OkResult>();

            var persistedHumidity = Context.Humidities.FirstOrDefault();
            persistedHumidity.Should().NotBeNull();
            persistedHumidity?.Percentage.Should().Be(validPercentage);
        }

        [Fact]
        public async Task GetCurrentHumidity_WithValidData_ShouldReturnOkResultWithHumidityDTO()
        {
            // arrange
            var expectedPercentage = 50;
            var humidityData = new HumidityBuilder().Build();
            Context.Humidities.Add(humidityData);

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
            var mockData = new List<Humidity>()
            {
                new HumidityBuilder().WithDate(startDate.AddMinutes(30)),
                new HumidityBuilder().WithDate(startDate.AddMinutes(90)),
                new HumidityBuilder().WithDate(endDate.AddHours(-2)),
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
                .BeCloseTo(mockData.ElementAt(0).Date, precision: TimeSpan.FromSeconds(1));
            humidityDtoList
                ?[1].Date.Should()
                .BeCloseTo(mockData.ElementAt(1).Date, precision: TimeSpan.FromSeconds(1));
            humidityDtoList
                ?[2].Date.Should()
                .BeCloseTo(mockData.ElementAt(2).Date, precision: TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task SetHumidity_WithInvalidPercentage_Should_ReturnBadRequestWithValidationErrors()
        {
            // arrange
            var invalidPercentage = -10;

            // act
            var result = await _controller.SetHumidity(invalidPercentage, SENSOR_TITLE);

            // assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            var errorMessage = badRequestResult?.Value as string;
            errorMessage.Should().Contain("Validation errors occurred");
        }
    }
}
