using Domain.Domain.Entities;
using FluentValidation;

namespace Domain.Domain.Validators;

public class TemperatureValidator : AbstractValidator<Temperature>
{
    public TemperatureValidator()
    {
        RuleFor(temperature => temperature.Celsius)
            .InclusiveBetween(10, 40)
            .WithMessage("Invalid temperature value. The temperature in Celsius should be between 10 and 40 degrees.");
    }
}
