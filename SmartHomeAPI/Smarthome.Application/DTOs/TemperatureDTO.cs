using Smarthome.Domain.Entities;

namespace Smarthome.Application.DTOs
{
    public class TemperatureDTO
    {
        public double Celsius { get; set; }
        public DateTime Date { get; set; }

        public static TemperatureDTO FromDomain(Temperature temperature) =>
            new TemperatureDTO { Celsius = temperature.Celsius, Date = temperature.Date };
    }
}
