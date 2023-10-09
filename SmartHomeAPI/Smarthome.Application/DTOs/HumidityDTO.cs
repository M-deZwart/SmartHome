using Smarthome.Domain.Entities;

namespace Smarthome.Application.DTOs
{
    public class HumidityDTO
    {
        public double Percentage { get; set; }
        public DateTime Date { get; set; }

        public static HumidityDTO FromDomain(Humidity humidity) =>
            new HumidityDTO { Percentage = humidity.Percentage, Date = humidity.Date };
    }
}
