using System.ComponentModel.DataAnnotations;

namespace SmartHomeAPI.Application.Entities
{
    public class Humidity
    {
        public Guid Id { get; set; }
        public double Percentage { get; set; }
        public DateTime Date { get; set; }
    }
}
