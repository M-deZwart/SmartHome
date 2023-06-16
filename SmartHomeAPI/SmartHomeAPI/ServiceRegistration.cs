using Infrastructure.Infrastructure;
using SmartHomeAPI.ApplicationCore.Interfaces;
using SmartHomeAPI.Interfaces;
using SmartHomeAPI.Mappers;

namespace SmartHomeAPI
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddTransient<IHumidityRepository, HumidityRepositoryEF>();
            services.AddTransient<ITemperatureRepository, TemperatureRepositoryEF>();
            services.AddTransient<IHumidityMapper, HumidityMapper>();
            services.AddTransient<ITemperatureMapper, TemperatureMapper>();

            return services;
        }
    }
}
