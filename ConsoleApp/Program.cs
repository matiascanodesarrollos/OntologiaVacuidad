using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Logging;
using DomainLogic.Services;
using System.Collections.Generic;
using DomainLogic.Services.ModelExtensions;
using DomainLogic.Services.Plasma;

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
            var designacion = AmbienteConfig.CrearAmbiente(logger);
            
            logger.LogInformation("═══ INICIANDO VIBRACIÓN ═══\n");
            var pendientes = new List<Designacion> { designacion };
            var interactionField = new PlasmaInteractionField();
            RgbColor? lastLoggedFieldColor = null;
            try
            {
                int iterationCount = 0;
                const int MaxIterations = 200; // Allow more iterations for full color progression
                
                while (pendientes.Count > 0 && iterationCount < MaxIterations)
                {
                    iterationCount++;
                    var designacionActual = pendientes[pendientes.Count - 1];
                    pendientes.RemoveAt(pendientes.Count - 1);
                    logger.LogInformation($"[STACK] Iteration={iterationCount} | Designacion={designacionActual} | pendiente={pendientes.Count}");

                    var causas = designacionActual.Apariencia.Significados;
                    for (int i = 0; i < causas.Count; i++)
                    {
                        var causa = causas[i];
                        _ = await causa.VibrarComoPlasma(mediator, logger, interactionField);
                    }

                    // Después de procesar TODAS las causas, agregar las nuevas designaciones encoladas
                    var nuevasDesignaciones = designacionQueue.DequeueAll();
                    if (nuevasDesignaciones.Count > 0)
                    {
                        logger.LogInformation($"\n[DESIGNACIONES-NUEVAS] Se agregaron {nuevasDesignaciones.Count} nuevas designaciones");
                        foreach (var nueva in nuevasDesignaciones)
                        {
                            pendientes.Add(nueva);
                            logger.LogInformation($"  → {nueva.Texto}");
                        }
                    }

                    var fieldEnergy = interactionField.ReadEnergy();
                    var fieldColor = PlasmaColor.FromFieldEnergy(fieldEnergy);
                    var fieldProgress = PlasmaColor.GetFieldProgress(fieldEnergy);
                    if (!lastLoggedFieldColor.HasValue || HasSignificantColorChange(lastLoggedFieldColor.Value, fieldColor))
                    {
                        logger.LogInformation(
                            $"[CAMPO-COLOR] Designacion={designacionActual.Texto} | " +
                            $"Color={fieldColor.ToHex()} ({PlasmaColor.Describe(fieldColor)}) | " +
                            $"Energia={fieldEnergy:F3} | Progreso={fieldProgress * 100:F1}%");
                        lastLoggedFieldColor = fieldColor;
                    }
                }
                logger.LogInformation("✓ Vibración de todas las designaciones completada");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error durante la vibración: {ex.Message}");
            }
        }

        private static bool HasSignificantColorChange(RgbColor previous, RgbColor current)
        {
            if (!string.Equals(PlasmaColor.GetColorFamily(previous), PlasmaColor.GetColorFamily(current), StringComparison.Ordinal))
            {
                return true;
            }

            int delta = Math.Abs(current.R - previous.R)
                      + Math.Abs(current.G - previous.G)
                      + Math.Abs(current.B - previous.B);
            return delta >= 20;
        }
    }
}