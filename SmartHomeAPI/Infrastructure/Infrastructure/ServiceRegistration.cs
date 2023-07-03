﻿using Application.Application.Interfaces;
using Infrastructure.Infrastructure.Logging;
using Infrastructure.Infrastructure.Mappers;
using Infrastructure.Infrastructure.Repositories;
using Interfaces.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                services.AddTransient<IHumidityMongoMapper, HumidityMongoMapper>();
                services.AddTransient<ITemperatureMongoMapper, TemperatureMongoMapper>();
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
            }

            services.AddScoped<IHumidityRepository, HumidityRepositoryEF>();
            services.AddScoped<ITemperatureRepository, TemperatureRepositoryEF>();

            services.AddScoped<IRequestLogger>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger>();
                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();

                return new RequestLogger(logger, httpContextAccessor);
            });

            return services;
        }
    }
}
