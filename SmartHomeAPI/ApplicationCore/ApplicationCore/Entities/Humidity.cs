using System.ComponentModel.DataAnnotations;

namespace SmartHomeAPI.ApplicationCore.Entities
{
    public class Humidity
    {
        [Key]
        public Guid ID { get; set; }

        public float Percentage { get; set; }

        public DateTime Date { get; set; }
    }
}
