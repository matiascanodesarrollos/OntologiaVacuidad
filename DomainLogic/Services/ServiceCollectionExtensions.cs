using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace DomainLogic.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDharmaServices(this IServiceCollection services)
        {
            services.AddLogging(configure =>
            {
                configure.AddConsole();
                configure.AddFile($"Logs/{DateTime.Now:yyyyMMdd_HHmmss}-vibracion.log");
            });
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));
            services.AddSingleton<ServiceConfig>();
            services.AddSingleton<IDesignacionQueue, DesignacionQueue>();
            return services;
        }
    }
}
