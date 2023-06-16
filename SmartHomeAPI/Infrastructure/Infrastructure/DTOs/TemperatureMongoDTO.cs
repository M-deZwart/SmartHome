using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructure.Infrastructure.DTOs
{
    public class TemperatureMongoDTO
    {
        [BsonId]
        public Guid ID { get; set; }

        public float Celsius { get; set; }

        public DateTime Date { get; set; }
    }
}
