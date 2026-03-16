using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
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
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var designacionQueue = serviceProvider.GetRequiredService<IDesignacionQueue>();
            var designacion = AmbienteConfig.CrearAmbiente();
            
            logger.LogInformation("═══ INICIANDO VIBRACIÓN DE PARTÍCULAS ═══\n");
            
            try
            {
                // Crear Espacio para la designación
                var espacio = new Espacio(designacion);
                logger.LogInformation($"[ESPACIO] Creado para: {designacion.Texto}\n");
                
                // Crear Electrons para cada naturaleza (efecto)
                var efectos = designacion.Apariencia.Efectos.ToList();
                var electrons = new List<Electron>();
                var trayectorias = new Dictionary<Electron, List<Vector2D>>();
                
                logger.LogInformation($"[ELECTRONS] Creando {efectos.Count} electrons...");
                for (int i = 0; i < efectos.Count; i++)
                {
                    var efecto = efectos[i];
                    var electron = new Electron(efecto, espacio);
                    
                    electrons.Add(electron);
                    trayectorias[electron] = new List<Vector2D> { new Vector2D(electron.Posicion2D.X, electron.Posicion2D.Y) };
                    logger.LogInformation($"  [{i}] {efecto.Naturaleza.Texto} - Fase: {efecto.Naturaleza.Fase * 180 / Math.PI:F1}°");
                }
                
                // Simular movimiento de los electrons
                const double deltaTime = 0.1; // 100ms por iteración
                const int maxIteraciones = 50;
                
                logger.LogInformation($"[SIMULACIÓN] Iniciando {maxIteraciones} iteraciones...\n");
                
                for (int iteracion = 0; iteracion < maxIteraciones; iteracion++)
                {
                    // Mover todos los electrons
                    foreach (var electron in electrons)
                    {
                        electron.Mover(deltaTime);
                        trayectorias[electron].Add(new Vector2D(electron.Posicion2D.X, electron.Posicion2D.Y));
                    }
                }
                
                logger.LogInformation($"\n[RESUMEN FINAL]");
                logger.LogInformation($"Espacio contiene {espacio.Particulas.Count} posiciones ocupadas");
                
                // Generar GIF animado mostrando evolución temporal
                var directorioSalida = Directory.GetCurrentDirectory();
                var gifLogger = serviceProvider.GetRequiredService<ILogger<GifGenerator>>();
                var gifGenerator = new GifGenerator(gifLogger);
                var gifPath = Path.Combine(directorioSalida, "trayectorias_electrons.gif");
                gifGenerator.GenerarGifAnimado(electrons, trayectorias, gifPath);
                logger.LogInformation($"\n✓ GIF animado generado: {gifPath}");
                
                logger.LogInformation("\n✓ Simulación completada exitosamente");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error durante la simulación: {ex.Message}");
            }
        }
    }
}