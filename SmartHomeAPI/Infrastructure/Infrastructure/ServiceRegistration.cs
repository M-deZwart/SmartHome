using Domain.Domain.Contracts;
using Infrastructure.Infrastructure.EF;
using Infrastructure.Infrastructure.Mongo;
using Infrastructure.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using SmartHomeAPI.Infrastructure.Repositories;

namespace Infrastructure.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var databaseType = configuration["DatabaseType"];

            if (databaseType is "Mongo")
            {
                var mongoConnectionString = configuration.GetConnectionString("MongoDB");
                var mongoClient = new MongoClient(mongoConnectionString);
                var mongoDatabase = mongoClient.GetDatabase("smarthome-db");
                services.AddSingleton(mongoDatabase);

                MongoDbConfig.Configure();

                services.AddScoped<MongoSeeder>();
                var serviceProvider = services.BuildServiceProvider();
                var mongoSeeder = serviceProvider.GetRequiredService<MongoSeeder>();
                mongoSeeder.SeedData();

                services.AddScoped<IHumidityRepository, HumidityRepositoryMongo>();
                services.AddScoped<ITemperatureRepository, TemperatureRepositoryMongo>();
            }
            if (databaseType is "EF")
            {
                services.AddDbContext<SmartHomeContext>(options =>
                {
                    var connectionString = configuration.GetConnectionString("EF");

                    options
                        .UseSqlite(connectionString)
                        .EnableSensitiveDataLogging()
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                });
                services.AddScoped<IHumidityRepository, HumidityRepositoryEF>();
                services.AddScoped<ITemperatureRepository, TemperatureRepositoryEF>();
            }

            return services;
        }
    }
}
