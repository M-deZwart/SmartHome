using Microsoft.Extensions.DependencyInjection;
using Application.Services;

namespace Application.Application
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ITemperatureService, TemperatureService>();
            services.AddScoped<IHumidityService, HumidityService>();

            return services;
        }
    }
}
