using Application.Application.Interfaces;
using Domain.Domain.Contracts;
using Infrastructure.Infrastructure.Logging;
using Infrastructure.Infrastructure.Mappers;
using Infrastructure.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SmartHomeAPI.Infrastructure.Repositories;

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

                services.AddTransient<IHumidityMongoMapper, HumidityMongoMapper>();
                services.AddTransient<ITemperatureMongoMapper, TemperatureMongoMapper>();

                services.AddScoped<IHumidityRepository, HumidityRepositoryMongo>();
                services.AddScoped<ITemperatureRepository, TemperatureRepositoryMongo>();
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
                services.AddScoped<IHumidityRepository, HumidityRepositoryEF>();
                services.AddScoped<ITemperatureRepository, TemperatureRepositoryEF>();
            }

            services.AddLogging();
            services.AddHttpContextAccessor();
            services.AddScoped<IRequestLogger>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<RequestLogger>>();
                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();

                return new RequestLogger(logger, httpContextAccessor);
            });

            return services;
        }
    }
}
