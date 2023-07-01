using System.ComponentModel.DataAnnotations;

namespace SmartHomeAPI.Application.Entities
{
    public class Temperature
    {
        public Guid Id { get; set; }
        public double Celsius { get; set; }
        public DateTime Date { get; set; }
    }
}
