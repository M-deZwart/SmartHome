namespace Smarthome.Domain.Entities
{
    public class Sensor
    {
        public Guid Id { get; private set; }
        public string Title { get; set; }
        public ICollection<Humidity> Humidities { get; set; } = new List<Humidity>();
        public ICollection<Temperature> Temperatures { get; set; } = new List<Temperature>();

        public Sensor(string title)
        {
            Title = title;
        }

        public static Sensor CreateSensor(string title) =>
            new Sensor(title) { Id = Guid.NewGuid(), };
    }
}
