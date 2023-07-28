using Application.Application.Contracts;
using Application.Application.DTOs;
using Application.Application.Services;
using Domain.Domain.Contracts;
using Domain.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Application.Tests.ServiceTests;
public class TemperatureServiceTests
{
    private readonly List<Temperature> _temperatureData;
    private readonly Mock<ITemperatureMapper> _mapper;
    private readonly Mock<ITemperatureRepository> _repository;
    private readonly TemperatureService _temperatureService;

    public TemperatureServiceTests()
    {
        _temperatureData = new List<Temperature>
            {
                new Temperature(celsius: 20, date: DateTime.UtcNow.AddDays(-1)),
                new Temperature(celsius: 30, date: DateTime.UtcNow),
                new Temperature (celsius : 15, date : DateTime.UtcNow.AddDays(-21))
            };

        _mapper = CreateMockTemperatureMapper();
        _repository = CreateMockTemperatureRepository(_temperatureData);
        _temperatureService = new TemperatureService(_repository.Object, _mapper.Object);
    }

    private Mock<ITemperatureRepository> CreateMockTemperatureRepository(List<Temperature> temperatureData)
    {
        var mockRepository = new Mock<ITemperatureRepository>();
        mockRepository
            .Setup(repo => repo.GetLatestTemperature())
            .ReturnsAsync(temperatureData.Last);

        mockRepository
            .Setup(repo => repo.GetByDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync((DateTime startDate, DateTime endDate) =>
            temperatureData.Where(temperature => temperature.Date >= startDate && temperature.Date <= endDate)
            .ToList());

        mockRepository
            .Setup(repo => repo.Create(It.IsAny<Temperature>()))
            .Returns(Task.CompletedTask);

        return mockRepository;
    }

    private Mock<ITemperatureMapper> CreateMockTemperatureMapper()
    {
        var mockMapper = new Mock<ITemperatureMapper>();
        mockMapper
            .Setup(mapper => mapper.MapToDTO(It.IsAny<Temperature>()))
            .Returns<Temperature>((temperature) => new TemperatureDTO
            {
                Celsius = temperature.Celsius,
                Date = temperature.Date
            });

        return mockMapper;
    }

    [Fact]
    public async Task GetCurrentTemperature_ShouldReturnCurrentTemperature()
    {
        // arrange
        var lastTemperature = _temperatureData.LastOrDefault();
        var expectedTemperature = lastTemperature is not null ? 
            _mapper.Object.MapToDTO(lastTemperature) : new TemperatureDTO();

        // act
        var result = await _temperatureService.GetCurrentTemperature();

        // assert   
        result.Should().BeEquivalentTo(expectedTemperature);
    }

    [Fact]
    public async Task GetTemperatureByDateRange_ShouldReturnTemperatureList()
    {
        // arrange
        var startDate = DateTime.UtcNow.AddDays(-2);
        var endDate = DateTime.UtcNow;

        // act
        var result = await _temperatureService.GetTemperatureByDateRange(startDate, endDate);

        // assert
        var expectedTemperatureList = _temperatureData
            .Where(temperature => temperature.Date >= startDate && temperature.Date <= endDate)
            .Select(_mapper.Object.MapToDTO)
            .ToList();

        result.Should().BeEquivalentTo(expectedTemperatureList);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task SetTemperature_ShouldCreateNewTemperature()
    {
        // arrange
        var celsius = 20;

        // act
        await _temperatureService.SetTemperature(celsius);

        // assert
        _repository.Verify(repo => repo.Create(It.Is<Temperature>(t => t.Celsius == celsius)), Times.Once);
    }
}
