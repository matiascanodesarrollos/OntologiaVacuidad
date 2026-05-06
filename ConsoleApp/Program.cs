using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DomainLogic.Services;
using DomainLogic.Services.Particulas;
using System.Collections.Generic;
using System.Linq;

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

            logger.LogInformation("═══ INICIANDO VIBRACIÓN DE PARTÍCULAS ═══\n");

            try
            {                
                var ambiente = AmbienteConfig.CrearAmbiente(string.Join(' ', args));
                
                logger.LogInformation($"[ESPACIO basado en amplitud (apariencia)] Creado para: {ambiente}\n");
                var espacio = new Espacio(ambiente);
                
                var framesDir = Path.Combine(Directory.GetCurrentDirectory(), "frames");
                var directorioSalida = Path.Combine(framesDir, "Amplitud");
                var paths = espacio.GenerarFramesPng(directorioSalida, 10, 30);
                foreach (var path in paths)
                {
                    logger.LogInformation($"Frame generado: {path}");
                }
                logger.LogInformation($"Espacio contiene {espacio.Particulas.Count} posiciones ocupadas");

                logger.LogInformation("\n✓ Simulación completada exitosamente");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error durante la simulación: {ex.Message}");
            }
        }
    }
}