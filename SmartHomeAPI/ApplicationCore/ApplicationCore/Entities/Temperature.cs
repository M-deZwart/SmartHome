using System.ComponentModel.DataAnnotations;

namespace SmartHomeAPI.ApplicationCore.Entities
{
    public class Temperature
    {
        public double Celsius { get; set; }

        public DateTime Date { get; set; }
    }
}
