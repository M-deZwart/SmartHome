using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructure.Infrastructure.DTOs
{
    public class HumidityMongoDTO
    {
        [BsonId]
        public Guid ID { get; set; }

        public float Percentage { get; set; }

        public DateTime Date { get; set; }
    }
}
