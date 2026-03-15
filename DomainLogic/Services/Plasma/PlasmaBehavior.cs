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
        private const double BaseSaturacionThreshold = 0.8;
        private const double BaseDesaparicionThreshold = 0.1;
        private const double BaseDamping = 0.02;
        private const double BaseDrivingForce = 1.80;
        private const double BaseDecayRate = 0.00005;

        /// <summary>
        /// Calcula dinámicamente el umbral de saturación basado en las propiedades espaciales del plasma.
        /// </summary>
        private double CalculateDynamicSaturacionThreshold(double posicion, double velocidad, double amplitud)
        {
            double posicionDistance = Math.Abs(posicion - 0.5) * 2;
            double posicionFactor = 1.0 - posicionDistance;
            double velocityFactor = velocidad > 0 ? 1.0 / (1.0 + Math.Log(velocidad + 1) * 0.5) : 1.0;
            double amplitudeFactor = Math.Clamp(amplitud, 0.0, 1.0);

            double dynamicThreshold = BaseSaturacionThreshold 
                * (0.7 + 0.3 * posicionFactor)
                * velocityFactor
                * (1.0 - amplitudeFactor * 0.1);

            return Math.Clamp(dynamicThreshold, 0.3, 0.95);
        }

        /// <summary>
        /// Calcula dinámicamente el umbral de desaparición basado en propiedades espaciales.
        /// - Posiciones extremas: umbral más bajo (se desvanecen más fácil)
        /// - Velocidad alta: umbral más bajo (menos estable)
        /// - Amplitud baja: umbral más bajo (tiende a desaparecer)
        /// </summary>
        private double CalculateDynamicDesaparicionThreshold(double posicion, double velocidad, double amplitud)
        {
            double posicionDistance = Math.Abs(posicion - 0.5) * 2;
            double posicionFactor = 1.0 - posicionDistance; // Centro es más estable

            // Velocidad alta reduce la estabilidad
            double velocityFactor = 1.0 - (velocidad > 0 ? Math.Min(Math.Log(velocidad + 1) * 0.1, 0.5) : 0);

            // Amplitud baja favorece desaparición
            double amplitudeFactor = Math.Pow(amplitud, 0.5); // Raíz cuadrada favorece valores bajos

            double dynamicThreshold = BaseDesaparicionThreshold 
                * (0.8 + 0.2 * posicionFactor)  // Variación: 80-100%
                * velocityFactor
                * (1.0 + amplitudeFactor * 0.2);

            return Math.Clamp(dynamicThreshold, 0.01, 0.4);
        }

        /// <summary>
        /// Calcula dinámicamente el amortiguamiento (damping) basado en propiedades espaciales.
        /// - Velocidad alta: más amortiguamiento (resistencia del medio)
        /// - Posición central: menos amortiguamiento (zona favorable)
        /// - Posiciones extremas: más amortiguamiento (zona desfavorable)
        /// </summary>
        private double CalculateDynamicDamping(double posicion, double velocidad, double direccion)
        {
            double posicionDistance = Math.Abs(posicion - 0.5) * 2;
            double posicionFactor = posicionDistance; // Factor inverso: extremos = más damping

            // Velocidad alta aumenta el damping (resistencia viscosa)
            double velocityInfluence = velocidad > 0 ? Math.Log(velocidad + 1) * 0.015 : 0;

            // La dirección puede modular levemente (si va contra la propagación natural)
            double direccionDeviation = Math.Abs(Math.Sin(direccion * 0.5));
            double direccionFactor = direccionDeviation * 0.005;

            double dynamicDamping = BaseDamping 
                * (1.0 + posicionFactor * 0.8)  // Variación: 100-180%
                + velocityInfluence
                + direccionFactor;

            return Math.Clamp(dynamicDamping, 0.005, 0.1);
        }

        /// <summary>
        /// Calcula dinámicamente la fuerza conductora basada en propiedades espaciales.
        /// - Velocidad alta: mayor fuerza conductora (impulso)
        /// - Posición central: fuerza estable
        /// - Amplitud inicial da contexto
        /// </summary>
        private double CalculateDynamicDrivingForce(double posicion, double velocidad, double amplitud, double sharedEnergy)
        {
            double posicionDistance = Math.Abs(posicion - 0.5) * 2;
            double posicionFactor = 1.0 - posicionDistance; // Centro favorece

            // Velocidad alta aumenta la fuerza conductora
            double velocityBoost = velocidad > 0 ? Math.Log(velocidad + 1) * 0.3 : 0;

            // La energía compartida también modula
            double energyInfluence = sharedEnergy * 0.2;

            // Amplitud baja puede significa que necesita más "empuje"
            double amplitudeCompensation = amplitud < 0.3 ? (0.3 - amplitud) * 0.5 : 0;

            double dynamicForce = BaseDrivingForce 
                * (0.9 + 0.2 * posicionFactor)    // Variación: 90-110%
                + velocityBoost
                + energyInfluence
                + amplitudeCompensation;

            return Math.Clamp(dynamicForce, 1.2, 2.8);
        }

        /// <summary>
        /// Calcula dinámicamente la tasa de decaimiento basada en propiedades espaciales.
        /// - Posiciones extremas: mayor decaimiento
        /// - Velocidad alta: menor decaimiento (energía en movimiento persiste)
        /// - Energía baja del campo: mayor decaimiento
        /// </summary>
        private double CalculateDynamicDecayRate(double posicion, double velocidad, double sharedEnergy)
        {
            double posicionDistance = Math.Abs(posicion - 0.5) * 2;
            // Posiciones extremas decaen más rápido
            double posicionFactor = posicionDistance * 0.0001;

            // Velocidad alta reduce el decaimiento (energía cinética contraresta)
            double velocityReduction = velocidad > 0 ? Math.Log(velocidad + 1) * 0.00005 : 0;

            // Energía compartida baja acelera el decaimiento
            double energyFactor = (1.0 - sharedEnergy) * 0.00003;

            double dynamicDecay = BaseDecayRate 
                + posicionFactor
                - velocityReduction
                + energyFactor;

            return Math.Clamp(dynamicDecay, 0.000001, 0.0002);
        }

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

            // Amplitud inicial dependiente de la apariencia del nombre
            double amplitude = nombre.Efecto.Amplitud / 55; // Normalizar a un rango inicial razonable
            
            // Capturas iniciales de las propiedades vectoriales del nombre
            double posicion = nombre.Posicion;
            double direccion = nombre.Direccion;
            double velocidad = nombre.Velocidad;
            
            const int steps = 120;
            const double dt = 0.04;

            for (int step = 0; step < steps; step++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                double sharedEnergy = field.ReadEnergy();
                
                // Calcular dinámicamente todos los factores basados en propiedades espaciales
                double dynamicDamping = CalculateDynamicDamping(posicion, velocidad, direccion);
                double dynamicDrivingForce = CalculateDynamicDrivingForce(posicion, velocidad, amplitude, sharedEnergy);
                double dynamicDecayRate = CalculateDynamicDecayRate(posicion, velocidad, sharedEnergy);
                
                // Modelo exponencial: crecimiento dependiente de la energía del campo y factores espaciales
                double velocityInfluence = velocidad > 0 ? Math.Log(velocidad + 1) * 0.1 : 0;
                double growthRate = dynamicDrivingForce + (sharedEnergy * 0.06) + velocityInfluence - (dynamicDamping * amplitude);
                
                // Actualizar amplitud con modelo exponencial suavizado
                amplitude = Math.Clamp(
                    amplitude * Math.Exp(growthRate * dt), 
                    0.0, 
                    1.0);

                // Aplicar decaimiento exponencial con respecto al tiempo (usando decay dinámico)
                amplitude *= Math.Exp(-dynamicDecayRate * step * dt);
                
                // Actualizar posición basada en velocidad y dirección
                posicion += velocidad * dt * Math.Cos(direccion);
                posicion = Math.Clamp(posicion, 0.0, 1.0);
                
                // Actualizar dirección suavemente hacia frecuencias dominantes
                direccion = (direccion + (velocidad * dt * 0.05)) % (2 * Math.PI);

                // Actualizar el campo compartido con esta amplitud
                field.Update(amplitude);
                
                // Registrar el vector de plasma en el campo
                field.UpdatePlasmaVector(nombre.Id.ToString(), posicion, direccion, velocidad);

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

                // Calcular umbral de saturación dinámico basado en propiedades espaciales
                double dynamicSaturacionThreshold = CalculateDynamicSaturacionThreshold(posicion, velocidad, amplitude);
                double dynamicDesaparicionThreshold = CalculateDynamicDesaparicionThreshold(posicion, velocidad, amplitude);

                if (!saturacionPublicada && amplitude >= dynamicSaturacionThreshold)
                {
                    saturacionPublicada = true;
                    saturaciones++;
                    await mediator.Publish(new SaturacionEvent(nombre), cancellationToken);
                    logger.LogWarning($"[SATURACION] {nombre.Texto} | paso={step} | amp={amplitude:F3} | umbral={dynamicSaturacionThreshold:F3} | pos={posicion:F3} | vel={velocidad:F3}");
                }

                if (!desaparicionPublicada && amplitude <= dynamicDesaparicionThreshold)
                {
                    desaparicionPublicada = true;
                    desapariciones++;
                    await mediator.Publish(new DesaparicionEvent(nombre), cancellationToken);
                    logger.LogWarning($"[DESAPARICION] {nombre.Texto} | paso={step} | amp={amplitude:F3} | umbral={dynamicDesaparicionThreshold:F3} | pos={posicion:F3} | vel={velocidad:F3}");
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
