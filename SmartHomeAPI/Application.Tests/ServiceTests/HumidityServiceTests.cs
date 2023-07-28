using Application.Application.Contracts;
using Application.Application.DTOs;
using Application.Application.Services;
using Domain.Domain.Contracts;
using Domain.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Application.Tests;

    public class HumidityServiceTests
    {
        private readonly List<Humidity> _humidityData;
        private readonly Mock<IHumidityMapper> _mapper;
        private readonly Mock<IHumidityRepository> _repository;
        private readonly HumidityService _humidityService;

        public HumidityServiceTests()
        {
            _humidityData = new List<Humidity>
            {
                new Humidity(percentage: 50, date: DateTime.UtcNow.AddDays(-1)),
                new Humidity(percentage: 60, date: DateTime.UtcNow),
                new Humidity(percentage: 70, date: DateTime.UtcNow.AddDays(-21))
            };

            _mapper = CreateMockHumidityMapper();
            _repository = CreateMockHumidityRepository(_humidityData);
            _humidityService = new HumidityService(_repository.Object, _mapper.Object);
        }

        private Mock<IHumidityRepository> CreateMockHumidityRepository(List<Humidity> humidityData)
        {
            var mockRepository = new Mock<IHumidityRepository>();
            mockRepository
                .Setup(repo => repo.GetLatestHumidity())
                .ReturnsAsync(humidityData.Last);

            mockRepository
                .Setup(repo => repo.GetByDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync((DateTime startDate, DateTime endDate) =>
                humidityData.Where(humidity => humidity.Date >= startDate && humidity.Date <= endDate)
                .ToList());

            mockRepository
                .Setup(repo => repo.Create(It.IsAny<Humidity>()))
                .Returns(Task.CompletedTask);

            return mockRepository;
        }

        private Mock<IHumidityMapper> CreateMockHumidityMapper()
        {
            var mockMapper = new Mock<IHumidityMapper>();
            mockMapper
                .Setup(mapper => mapper.MapToDTO(It.IsAny<Humidity>()))
                .Returns<Humidity>((humidity) => new HumidityDTO
                {
                    Percentage = humidity.Percentage,
                    Date = humidity.Date
                });

            return mockMapper;
        }

        [Fact]
        public async Task GetCurrentHumidity_ShouldReturnCurrentHumidity()
        {
            // arrange
            var lastHumidity = _humidityData.LastOrDefault();
            var expectedHumidity = lastHumidity is not null ? 
                _mapper.Object.MapToDTO(lastHumidity) : new HumidityDTO();

            // act
            var result = await _humidityService.GetCurrentHumidity();

            // assert   
            result.Should().BeEquivalentTo(expectedHumidity);
        }

        [Fact]
        public async Task GetHumidityByDateRange_ShouldReturnHumidityList()
        {
            // arrange
            var startDate = DateTime.UtcNow.AddDays(-2);
            var endDate = DateTime.UtcNow;

            // act
            var result = await _humidityService.GetHumidityByDateRange(startDate, endDate);

            // assert
            var expectedHumidityList = _humidityData
                .Where(humidity => humidity.Date >= startDate && humidity.Date <= endDate)
                .Select(_mapper.Object.MapToDTO)
                .ToList();

            result.Should().BeEquivalentTo(expectedHumidityList);
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task SetHumidity_ShouldCreateNewHumidity()
        {
            // arrange
            var percentage = 75;

            // act
            await _humidityService.SetHumidity(percentage);

            // assert
            _repository.Verify(repo => repo.Create(It.Is<Humidity>(h => h.Percentage == percentage)), Times.Once);
        }
    }

