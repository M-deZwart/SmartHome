using SmartHomeAPI.ApplicationCore.Interfaces;
using SmartHomeAPI.Infrastructure;

namespace SmartHomeAPI
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddTransient<IHumidityRepository, HumidityRepositoryMongo>();
            services.AddTransient<ITemperatureRepository, TemperatureRepositoryMongo>();

            return services;
        }
    }
}
