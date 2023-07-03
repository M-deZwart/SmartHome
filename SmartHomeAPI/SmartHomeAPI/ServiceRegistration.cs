using SmartHomeAPI.MappersAPI;
using SmartHomeAPI.Mappers;
using Interfaces.Interfaces;
using Infrastructure.Infrastructure.Repositories;
using ApplicationCore.ApplicationCore.Interfaces;

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
