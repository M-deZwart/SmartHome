﻿using Smarthome.Domain.Exceptions;
using Smarthome.Domain.Validators;
using System.Text;

namespace Smarthome.Domain.Entities
{
    public class Temperature
    {
        public Guid Id { get; private set; }
        public double Celsius { get; private set; }
        public DateTime Date { get; private set; }

        public Guid SensorId { get; set; }
        public Sensor Sensor { get; set; } = null!;

        public Temperature(double celsius, DateTime date)
        {
            Celsius = celsius;
            Date = date;

            Validate();
        }

        private void Validate()
        {
            var validator = new TemperatureValidator();
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
