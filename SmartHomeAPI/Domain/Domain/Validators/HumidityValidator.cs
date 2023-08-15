using Domain.Domain.Entities;
using FluentValidation;

namespace Domain.Domain.Validators;

public class HumidityValidator : AbstractValidator<Humidity>
{
    public HumidityValidator()
    {
        RuleFor(humidity => humidity.Percentage).InclusiveBetween(1, 100).WithMessage("Invalid percentage value. The humidity percentage should be between 0 and 100.");
        RuleFor(humidity => humidity.Date)
            .Must((date) => date <= DateTime.Now)
            .WithMessage("Date cannot be in the future.");
    }
}
