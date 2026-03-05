using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainLogic.Services;
using DomainLogic.Services.Events;
using DomainLogic.Services.Ondas;
using MediatR;
using Microsoft.Extensions.Logging;

public static class NombreExtension
{
    // Contexto espacial por nombre (temporal durante vibración)
    private static readonly ConcurrentDictionary<Guid, double> _nombrePositions = 
        new ConcurrentDictionary<Guid, double>();

    /// <summary>
    /// Establece la posición en el espacio del nombre para acoplamiento
    /// </summary>
    public static void SetPosition(this Nombre nombre, double position)
    {
        _nombrePositions.AddOrUpdate(nombre.Id, position, (k, v) => position);
    }

    /// <summary>
    /// Obtiene la posición actual del nombre en el espacio
    /// </summary>
    public static double GetPosition(this Nombre nombre)
    {
        return _nombrePositions.TryGetValue(nombre.Id, out var pos) ? pos : 0.0;
    }

    /// <summary>
    /// Limpia la posición (al finalizar vibración)
    /// </summary>
    public static void ClearPosition(this Nombre nombre)
    {
        _nombrePositions.TryRemove(nombre.Id, out _);
    }

    /// <summary>
    /// Simula vibración natural del nombre en un ambiente específico
    /// Emite eventos de Saturación y Desaparición según corresponda
    /// Soporta acoplamiento de ondas con otros nombres en el espacio compartido
    /// </summary>
    public static async Task Vibrar(this Nombre nombre, IMediator mediator, ILogger logger)
    {
        const double AmplitudSaturacionThreshold = 0.8;   // 80% de amplitud = saturación
        const double AmplitudDesaparicionThreshold = 0.1; // 10% de amplitud = desaparición

        try
        {
            // Obtener los 4 pilotos del ambiente (Tierra, Agua, Aire, Fuego)
            var tierra = Designacion.Crear("Sólida", "Tierra", "Estar", Math.PI / 2, 1000);
            var agua = Designacion.Crear("Fluida", "Agua", "Ser", Math.PI / 2, 400);
            var aire = Designacion.Crear("Disperso", "Aire", "Existir", Math.PI, 600);
            var fuego = Designacion.Crear("Caliente", "Fuego", "Transformar", 3 * Math.PI / 2, 200);

            var pilotos = new List<Designacion> { tierra, agua, aire, fuego };

            // Crear un frame OFDM base para la vibración
            var designacion = nombre.Efecto.Esencia;
            var frame = designacion.CrearOfdmFrame(pilotos);

            // Rastrear estado previo de amplitudes para detectar transiciones
            var estadoPrevio = new Dictionary<int, (bool saturada, bool desaparecida)>();
            for (int i = 0; i < frame.Count; i++)
            {
                estadoPrevio[i] = (false, false);
            }

            // Simular evolución de vibración durante N iteraciones
            int iteraciones = 50;
            for (int iter = 0; iter < iteraciones; iter++)
            {
                // Evolucionar amplitud de cada subportadora
                for (int i = 0; i < frame.Count; i++)
                {
                    var signal = frame[i];
                    var (wasSaturada, wasDesaparecida) = estadoPrevio[i];

                    bool esSaturada = signal.Amplitude >= AmplitudSaturacionThreshold;
                    bool esDesaparecida = signal.Amplitude <= AmplitudDesaparicionThreshold;

                    // Transición a saturación
                    if (esSaturada && !wasSaturada && !esDesaparecida)
                    {
                        logger?.LogWarning($"[SATURACIÓN] {nombre.Texto} - Subportadora {i}: Amplitud = {signal.Amplitude:F3}");
                        await mediator.Publish(new SaturacionEvent(nombre));
                    }

                    // Transición a desaparición
                    if (esDesaparecida && !wasDesaparecida)
                    {
                        logger?.LogWarning($"[DESAPARICIÓN] {nombre.Texto} - Subportadora {i}: Amplitud = {signal.Amplitude:F3}");
                        await mediator.Publish(new DesaparicionEvent(nombre));
                    }

                    // Recordar estado
                    estadoPrevio[i] = (esSaturada, esDesaparecida);

                    // Evolucionar amplitud de forma realista (crecimiento -> pico -> decaimiento)
                    double progreso = iter / (double)iteraciones;
                    if (progreso < 0.3)
                    {
                        // Crecimiento rápido
                        signal.Amplitude = Math.Min(1.0, signal.Amplitude + 0.015);
                    }
                    else if (progreso < 0.7)
                    {
                        // Mantener pico con oscilación
                        double oscilacion = Math.Sin(iter * 0.2) * 0.02;
                        signal.Amplitude = Math.Min(0.95, signal.Amplitude + oscilacion);
                    }
                    else
                    {
                        // Decañimiento lento
                        signal.Amplitude = Math.Max(0.0, signal.Amplitude - 0.01);
                    }
                }

                // Pequeño delay para simular procesamiento
                await Task.Delay(50);
            }

            logger?.LogInformation($"Vibración de {nombre.Texto} completada");
        }
        catch (Exception ex)
        {
            logger?.LogError($"Error durante vibración de {nombre.Texto}: {ex.Message}");
        }
    }
}