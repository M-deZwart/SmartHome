using Domain.Domain.Entities;

namespace Domain.Tests.Builders;

public class TemperatureBuilder
{
    public static implicit operator Temperature(TemperatureBuilder builder) => builder.Build();

    private double _celsius = 20;
    private DateTime _date = DateTime.Now;

    public TemperatureBuilder WithCelsius(double celsius)
    {
        _celsius = celsius;
        return this;
    }

    public TemperatureBuilder WithDate(DateTime date)
    {
        _date = date;
        return this;
    }

    public Temperature Build()
    {
        return new Temperature(_celsius, _date);
    }
}
