using Microsoft.Extensions.DependencyInjection;
using Application.Application.Services;
using Application.Application.Contracts;
using Application.Application.ApiMappers;

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
