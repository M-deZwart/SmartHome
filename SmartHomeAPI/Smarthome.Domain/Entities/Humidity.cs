using Smarthome.Domain.Exceptions;
using Smarthome.Domain.Validators;
using System.Text;

namespace Smarthome.Domain.Entities
{
    public class Humidity
    {
        public Guid Id { get; set; }
        public double Percentage { get; private set; }
        public DateTime Date { get; private set; }

        public Guid SensorId { get; set; }
        public Sensor Sensor { get; set; } = null!;

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
