using Domain.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Infrastructure.EF
{
    public class SmartHomeContext : DbContext
    {
        public DbSet<Temperature> Temperatures { get; set; }
        public DbSet<Humidity> Humidities { get; set; }
        public DbSet<Sensor> Sensors { get; set; }

        public SmartHomeContext(DbContextOptions<SmartHomeContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Temperature>()
                .ToTable("Temperatures")
                .HasKey(t => t.Id);

            modelBuilder.Entity<Temperature>()
                .Property(t => t.Celsius)
                .HasColumnName(nameof(Temperature.Celsius));

            modelBuilder.Entity<Temperature>()
                .Property(t => t.Date)
                .HasColumnName(nameof(Temperature.Date));

            modelBuilder.Entity<Humidity>()
                .ToTable("Humidities")
                .HasKey(h => h.Id);

            modelBuilder.Entity<Humidity>()
                .Property(h => h.Percentage)
                .HasColumnName(nameof(Humidity.Percentage));

            modelBuilder.Entity<Humidity>()
                .Property(h => h.Date)
                .HasColumnName(nameof(Humidity.Date));

            modelBuilder.Entity<Temperature>()
                .HasOne(t => t.Sensor)
                .WithMany(s => s.Temperatures)
                .HasForeignKey(t => t.SensorId);

            modelBuilder.Entity<Humidity>()
                .HasOne(h => h.Sensor)
                .WithMany(s => s.Humidities)
                .HasForeignKey(h => h.SensorId);

            modelBuilder.Entity<Sensor>().HasData(
                    new Sensor(title: "LivingRoom") { Id = Guid.NewGuid() });
            modelBuilder.Entity<Sensor>().HasData(
                    new Sensor(title: "Bedroom") { Id = Guid.NewGuid() });
            modelBuilder.Entity<Sensor>().HasData(
                    new Sensor(title: "WorkSpace") { Id = Guid.NewGuid() });

            base.OnModelCreating(modelBuilder);
        }

    }
}
