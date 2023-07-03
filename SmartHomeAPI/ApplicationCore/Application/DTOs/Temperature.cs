using System.ComponentModel.DataAnnotations;

namespace Application.Application.DTOs
{
    public class Temperature
    {
        public Guid Id { get; set; }
        public double Celsius { get; set; }
        public DateTime Date { get; set; }
    }
}
