using Smarthome.Domain.Entities;
using FluentValidation;

namespace Smarthome.Domain.Validators;

public class TemperatureValidator : AbstractValidator<Temperature>
{
    public TemperatureValidator()
    {
        RuleFor(temperature => temperature.Celsius)
            .InclusiveBetween(10, 40)
            .WithMessage(
                "Invalid Celsius value. The temperature in Celsius should be between 10 and 40 degrees."
            );
        RuleFor(humidity => humidity.Date)
            .LessThanOrEqualTo(date => DateTime.Now)
            .WithMessage("Date cannot be in the future.");
    }
}
