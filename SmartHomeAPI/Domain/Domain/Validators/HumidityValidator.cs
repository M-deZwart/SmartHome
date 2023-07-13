using Domain.Domain.Entities;
using FluentValidation;

namespace Domain.Domain.Validators;
public class HumidityValidator : AbstractValidator<Humidity>
{
    public HumidityValidator()
    {
        RuleFor(humidity => humidity.Percentage)
            .InclusiveBetween(0, 100)
            .WithMessage("Invalid humidity value. The humidity percentage should be between 0 and 100.");
    }
}
