using Domain.Domain.Exceptions;
using Domain.Domain.Validators;
using System.Text;

namespace Domain.Domain.Entities
{
    public class Humidity
    {
        public Guid Id { get; set; }
        public double Percentage { get; private set; }
        public DateTime Date { get; private set; }

        public Humidity(double percentage, DateTime date)
        {
            Percentage = percentage;
            Date = date;

            Validate();
        }

        private void Validate()
        {
            var validator = new HumidityValidator();
            var validationresult = validator.Validate(this);
            if (!validationresult.IsValid)
            {
                var errormessages = new StringBuilder();
                foreach (var error in validationresult.Errors)
                {
                    errormessages.AppendLine(error.ErrorMessage);
                }

                throw new DomainException(errormessages.ToString());
            }
        }
    }
}
