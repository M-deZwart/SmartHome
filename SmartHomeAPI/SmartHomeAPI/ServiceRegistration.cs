using SmartHomeAPI.ApplicationCore.Interfaces;
using SmartHomeAPI.Infrastructure;

namespace SmartHomeAPI
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IHumidityRepository, HumidityRepository>();
            services.AddTransient<ITemperatureRepository, TemperatureRepository>();

            return services;
        }
    }
}
