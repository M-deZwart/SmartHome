using System.ComponentModel.DataAnnotations;

namespace SmartHomeAPI.ApplicationCore.Entities
{
    public class Temperature
    {
        [Key]
        public Guid ID { get; set; }

        public float Celsius { get; set; }

        public DateTime Date { get; set; }
    }
}
