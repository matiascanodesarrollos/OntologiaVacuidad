using System;
using System.Threading;
using System.Threading.Tasks;
using DomainLogic.Services.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DomainLogic.Services.Plasma
{
    public class PlasmaBehavior
    {
        private const double SaturacionThreshold = 0.8;
        private const double DesaparicionThreshold = 0.1;
        private const double Damping = 0.02;
        private const double DrivingForce = 1.80;

        public async Task<PlasmaRunResult> ExecuteAsync(
            Nombre nombre,
            IMediator mediator,
            ILogger logger,
            PlasmaInteractionField field,
            CancellationToken cancellationToken = default)
        {
            bool saturacionPublicada = false;
            bool desaparicionPublicada = false;

            int saturaciones = 0;
            int desapariciones = 0;
            RgbColor? lastLoggedFieldColor = null;

            // Amplitud inicial simple
            double amplitude = 0.11;
            
            const int steps = 120;
            const double dt = 0.04;

            for (int step = 0; step < steps; step++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                double sharedEnergy = field.ReadEnergy();
                
                // Modelo exponencial simple: crecimiento dependiente de la energía del campo
                // factor de crecimiento: energía compartida puede acelerar o desacelerar
                double growthRate = DrivingForce + (sharedEnergy * 0.06) - (Damping * amplitude);
                
                // Actualizar amplitud con modelo exponencial suavizado
                amplitude = Math.Clamp(
                    amplitude * Math.Exp(growthRate * dt), 
                    0.0, 
                    1.0);

                // Actualizar el campo compartido con esta amplitud
                field.Update(amplitude, amplitude);

                if (step % 4 == 0)
                {
                    double fieldEnergy = field.ReadEnergy();
                    var fieldColor = PlasmaColor.FromFieldEnergy(fieldEnergy);
                    var fieldProgress = PlasmaColor.GetFieldProgress(fieldEnergy);
                    if (!lastLoggedFieldColor.HasValue || HasSignificantColorChange(lastLoggedFieldColor.Value, fieldColor))
                    {
                        logger.LogInformation(
                            $"[CAMPO-EVOLUCION] Causa={nombre.Texto} | paso={step} | " +
                            $"Color={fieldColor.ToHex()} ({PlasmaColor.Describe(fieldColor)}) | " +
                            $"Energia={fieldEnergy:F3} | Progreso={(fieldProgress * 100):F1}%");
                        lastLoggedFieldColor = fieldColor;
                    }
                }

                if (!saturacionPublicada && amplitude >= SaturacionThreshold)
                {
                    saturacionPublicada = true;
                    saturaciones++;
                    await mediator.Publish(new SaturacionEvent(nombre), cancellationToken);
                    logger.LogWarning($"[SATURACION] {nombre.Texto} | paso={step} | amp={amplitude:F3}");
                }

                if (!desaparicionPublicada && amplitude <= DesaparicionThreshold)
                {
                    desaparicionPublicada = true;
                    desapariciones++;
                    await mediator.Publish(new DesaparicionEvent(nombre), cancellationToken);
                    logger.LogWarning($"[DESAPARICION] {nombre.Texto} | paso={step} | amp={amplitude:F3}");
                }
            }

            var color = PlasmaColor.FromFieldEnergy(field.ReadEnergy());

            return new PlasmaRunResult(
                Color: color,
                Saturaciones: saturaciones,
                Desapariciones: desapariciones,
                AmplitudPromedioFinal: amplitude,
                AmplitudMaximaFinal: amplitude);
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
