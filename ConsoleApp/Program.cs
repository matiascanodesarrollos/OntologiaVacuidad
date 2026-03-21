using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DomainLogic.Services;
using DomainLogic.Services.Particulas;

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
            var ambiente = AmbienteConfig.CrearAmbiente();
            
            logger.LogInformation("═══ INICIANDO VIBRACIÓN DE PARTÍCULAS ═══\n");
            
            try
            {
                logger.LogInformation($"[ESPACIO] Creado para: {ambiente.NaturalezaAparente.Causa}\n");                
                var espacio = Espacio.Crear(ambiente.NaturalezaAparente.Causa);
                                
                var directorioSalida = Directory.GetCurrentDirectory();
                var paths = espacio.GenerarFramesPng(directorioSalida, 4, 500);
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