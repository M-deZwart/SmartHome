using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Infrastructure.DTOs
{
    public class HumidityEfDTO
    {
        [Key]
        public Guid ID { get; set; }

        public double Percentage { get; set; }

        public DateTime Date { get; set; }
    }
}
