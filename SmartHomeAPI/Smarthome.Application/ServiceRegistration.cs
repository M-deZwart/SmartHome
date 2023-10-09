using Microsoft.Extensions.DependencyInjection;
using Application.Services;
using Smarthome.Application.Contracts;
using Smarthome.Application.ApiMappers;

namespace Application.Application
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddTransient<IHumidityMapper, HumidityMapper>();
            services.AddTransient<ITemperatureMapper, TemperatureMapper>();
            services.AddScoped<ITemperatureService, TemperatureService>();
            services.AddScoped<IHumidityService, HumidityService>();

            return services;
        }
    }
}
