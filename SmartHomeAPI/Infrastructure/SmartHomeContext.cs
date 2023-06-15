using Microsoft.EntityFrameworkCore;
using SmartHomeAPI.ApplicationCore.Entities;

namespace Infrastructure
{
    public class SmartHomeContext : DbContext
    {
        public DbSet<Temperature> Temperatures { get; set; }
        public DbSet<Humidity> Humidities { get; set; }
    }
}
