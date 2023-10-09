using Smarthome.Domain.Entities;

namespace Domain.Tests.Builders;

public class HumidityBuilder
{
    public static implicit operator Humidity(HumidityBuilder builder) => builder.Build();

    private double _percentage = 50;
    private DateTime _date = DateTime.Now;

    public HumidityBuilder WithPercentage(double percentage)
    {
        _percentage = percentage;
        return this;
    }

    public HumidityBuilder WithDate(DateTime date)
    {
        _date = date;
        return this;
    }

    public Humidity Build()
    {
        return new Humidity(_percentage, _date);
    }
}
