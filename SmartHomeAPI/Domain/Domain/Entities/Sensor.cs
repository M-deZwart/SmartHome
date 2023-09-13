
namespace Domain.Domain.Entities
{
    public class Sensor
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public ICollection<Humidity> Humidities { get; } = new List<Humidity>();
        public ICollection<Temperature> Temperatures { get; } = new List<Temperature>();

        public Sensor(string title)
        {
            Title = title;
        }
    }
}
