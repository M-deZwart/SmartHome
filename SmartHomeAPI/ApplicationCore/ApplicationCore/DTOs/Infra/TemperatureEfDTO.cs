using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Infrastructure.DTOs
{
    public class TemperatureEfDTO
    {
        [Key]
        public Guid ID { get; set; }

        public double Celsius { get; set; }

        public DateTime Date { get; set; }
    }
}
