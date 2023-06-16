using Infrastructure.Infrastructure.DTOs;
using Microsoft.EntityFrameworkCore;
using SmartHomeAPI.ApplicationCore.Entities;

namespace Infrastructure.Infrastructure
{
    public class SmartHomeContext : DbContext
    {
        public DbSet<TemperatureEfDTO> Temperatures { get; set; }
        public DbSet<HumidityEfDTO> Humidities { get; set; }

        public SmartHomeContext(DbContextOptions<SmartHomeContext> options)
            :base(options)
        {
            
        }
    }
}
