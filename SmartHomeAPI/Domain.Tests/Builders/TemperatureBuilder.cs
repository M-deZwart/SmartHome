using Domain.Domain.Entities;

namespace Domain.Tests.Builders;

public class TemperatureBuilder
{
    public static implicit operator Temperature(TemperatureBuilder builder) => builder.Build();

    private Guid _id;
    private double _celsius;
    private DateTime _date;

    public TemperatureBuilder()
    {
        _id = Guid.NewGuid();
        _celsius = 0;
        _date = DateTime.Now;
    }

    public TemperatureBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

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
        return new Temperature
        {
            Id = _id,
            Celsius = _celsius,
            Date = _date
        };
    }
}
