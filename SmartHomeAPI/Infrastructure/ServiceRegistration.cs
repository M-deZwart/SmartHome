using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var databaseType = configuration["DatabaseType"];

            if (databaseType == "Mongo")
            {
                var mongoConnectionString = configuration.GetConnectionString("MongoDB");
                var mongoClient = new MongoClient(mongoConnectionString);
                var mongoDatabase = mongoClient.GetDatabase("smarthome-db");
                services.AddSingleton(mongoDatabase);
            }
            if (databaseType == "EF")
            {
                services.AddDbContext<SmartHomeContext>(options =>
                {
                    var connectionString = configuration.GetConnectionString("EF");
                    options.UseSqlServer(connectionString)
                    .EnableSensitiveDataLogging()
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); 
                });
            }
            return services;
        }
    }
}
