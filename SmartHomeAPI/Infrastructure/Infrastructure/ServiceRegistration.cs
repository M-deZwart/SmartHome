using ApplicationCore.ApplicationCore.Interfaces.InfraMappers;
using Infrastructure.Infrastructure.DTOs;
using Infrastructure.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var databaseType = configuration["DatabaseType"];

            if (databaseType is "Mongo")
            {
                var mongoConnectionString = configuration.GetConnectionString("MongoDB");
                var mongoClient = new MongoClient(mongoConnectionString);
                var mongoDatabase = mongoClient.GetDatabase("smarthome-db");
                services.AddSingleton(mongoDatabase);
            }
            if (databaseType is "EF")
            {
                services.AddDbContext<SmartHomeContext>(options =>
                {
                    var connectionString = configuration.GetConnectionString("EF");
                    options.UseSqlServer(connectionString)
                    .EnableSensitiveDataLogging()
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                });
                services.AddTransient<IHumidityMapper<HumidityEfDTO>, HumidityEfMapper>();
                services.AddTransient<ITemperatureMapper<TemperatureEfDTO>, TemperatureEfMapper>();
            }       
            return services;
        }
    }
}
