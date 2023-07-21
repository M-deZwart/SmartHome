using Domain.Domain.Entities;

namespace Domain.Tests.Builders;

public class HumidityBuilder
{
    public static implicit operator Humidity(HumidityBuilder builder) => builder.Build();

    private Guid _id;
    private double _percentage;
    private DateTime _date;

    public HumidityBuilder()
    {
        _id = Guid.NewGuid();
        _percentage = 50;
        _date = DateTime.Now;
    }

    public HumidityBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

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
        return new Humidity
        {
            Id = _id,
            Percentage = _percentage,
            Date = _date
        };
    }
}
