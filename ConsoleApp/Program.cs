using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Logging;
using DomainLogic.Services;

namespace ConsoleApp
{
    class Program
    {
        
        static async Task Main(string[] args)
        {
            // Configuración DI con extensión
            var services = new ServiceCollection();
            services.AddDharmaServices();
            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            var designacion = AmbienteConfig.CrearAmbiente(logger);
            
            logger.LogInformation("═══ INICIANDO VIBRACIÓN ═══\n");
            try
            {
                await designacion.Nombre.Vibrar(
                    serviceProvider.GetRequiredService<IMediator>(), 
                    logger);
                logger.LogInformation("✓ Vibración de todas las designaciones completada");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error durante la vibración: {ex.Message}");
            }
        }
    }
}