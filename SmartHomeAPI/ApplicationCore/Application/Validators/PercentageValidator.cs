using FluentValidation;

namespace Application.Application.Validators;
public class PercentageValidator : AbstractValidator<double>
{
    // in domeinobject class constructor en dan exception gooien als die niet klopt domainexception
    public PercentageValidator()
    {
        RuleFor(percentage => percentage)
            .InclusiveBetween(0, 100)
            .WithMessage("Invalid percentage value. The humidity percentage should be between 0 and 100.");
    }
}
