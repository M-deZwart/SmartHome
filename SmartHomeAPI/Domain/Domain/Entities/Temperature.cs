
using Domain.Domain.Exceptions;
using Domain.Domain.Validators;
using System.Text;

namespace Domain.Domain.Entities
{
    public class Temperature
    {
        public Guid Id { get; set; }
        public double Celsius { get; set; }
        public DateTime Date { get; set; }

        public Temperature()
        {
            var validator = new TemperatureValidator();
            var validationResult = validator.Validate(this);
            if (!validationResult.IsValid)
            {
                var errorMessages = new StringBuilder();
                foreach (var error in validationResult.Errors)
                {
                    errorMessages.AppendLine(error.ErrorMessage);
                }

                throw new DomainException(errorMessages.ToString());
            }
        }
    }
}
