using FluentValidation;

namespace Smarthome.Application.Validators;

public class CelsiusValidator : AbstractValidator<double>
{
    public CelsiusValidator()
    {
        RuleFor(celsius => celsius)
            .InclusiveBetween(10, 40)
            .WithMessage(
                "Invalid Celsius value. The temperature in Celsius should be between 10 and 40 degrees."
            );
    }
}
