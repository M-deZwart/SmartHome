using SmartHomeAPI.MappersAPI;
using SmartHomeAPI.Mappers;
using Interfaces.Interfaces;
using Infrastructure.Infrastructure.Repositories;
using SmartHomeAPI.Interfaces;
using SmartHomeAPI.Services;
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

            services.AddScoped<IRequestLogger>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger>();
                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();

                return new RequestLoggerService(logger, httpContextAccessor);
            });
            return services;
        }
    }
}
