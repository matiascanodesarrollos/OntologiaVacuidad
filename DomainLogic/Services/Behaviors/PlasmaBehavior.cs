using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainLogic.Services.Events;
using DomainLogic.Services.Ondas;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DomainLogic.Services.Behaviors
{
    /// <summary>
    /// Comportamiento tipo plasma: sistema de EDOs acopladas que modelan
    /// dinámicas no lineales con interacción entre subportadoras OFDM.
    /// Usa MathNet.Numerics para resolver la evolución temporal realista.
    /// Considera la cadena causa-efecto de las Designaciones como sub-plasmas que interactúan.
    /// </summary>
    public class PlasmaBehavior : IBehavior
    {
        private const double AmplitudSaturacionThreshold = 0.8;
        private const double AmplitudDesaparicionThreshold = 0.1;
        private const double DampingCoefficient = 0.08;      // Amortiguamiento viscoso
        private const double NonlinearityCoefficient = 0.4;  // Saturación no lineal
        private const double CouplingStrength = 0.15;        // Fuerza de acoplamiento entre subportadoras
        private const double DrivingForce = 0.08;           // Fuerza impulsora externa

        /// <summary>
        /// Color RGB actual del plasma (resultado de última simulación)
        /// </summary>
        public RgbColor ColorActual { get; private set; }

        public async Task Execute(Nombre nombre, IMediator mediator, ILogger logger)
        {
            try
            {
                // Crear frame OFDM base - estructura de 64 subportadoras
                var designacion = nombre.Efecto.Esencia;
                var frame = designacion.CrearOfdmFrame();

                logger.LogInformation($"[PLASMA] Iniciando simulación para {nombre.Texto} con {frame.Count} subportadoras");

                // Estado inicial: amplitudes y fases de cada subportadora
                int numSubcarriers = frame.Count;
                double[] amplitudes = new double[numSubcarriers];
                double[] phases = new double[numSubcarriers];
                double[] frequencies = new double[numSubcarriers];

                for (int i = 0; i < numSubcarriers; i++)
                {
                    amplitudes[i] = frame[i].Amplitude > 0 ? frame[i].Amplitude : 0.1;
                    phases[i] = frame[i].Phase;
                    frequencies[i] = frame[i].Frequency;
                }

                // Rastrear transiciones de estado
                var estadoPrevio = new Dictionary<int, (bool saturada, bool desaparecida)>();
                for (int i = 0; i < numSubcarriers; i++)
                {
                    estadoPrevio[i] = (false, false);
                }

                // Simulación temporal: resolver sistema de EDOs acopladas
                double t = 0.0;
                double dt = 0.05;  // paso de tiempo
                int pasosSimulacion = 100;
                int countSaturacion = 0, countDesaparicion = 0;
                
                // Tracking de cambios de color
                var colorAnterior = new RgbColor { R = 0, G = 0, B = 0 };
                string categoriaAnterior = "";

                for (int paso = 0; paso < pasosSimulacion; paso++)
                {
                    // Estado vectorizado del plasma: [A_0, A_1, ..., A_63, φ_0, φ_1, ..., φ_63]
                    double[] y = new double[numSubcarriers * 2];
                    Buffer.BlockCopy(amplitudes, 0, y, 0, numSubcarriers * sizeof(double));
                    Buffer.BlockCopy(phases, 0, y, numSubcarriers * sizeof(double), numSubcarriers * sizeof(double));

                    // Definir sistema de EDOs: dA_i/dt y dφ_i/dt para cada subportadora
                    Action<double, double[], double[]> dydt = (time, state, deriv) =>
                    {
                        // Extraer amplitudes y fases del estado
                        var A = new double[numSubcarriers];
                        var phi = new double[numSubcarriers];
                        Buffer.BlockCopy(state, 0, A, 0, numSubcarriers * sizeof(double));
                        Buffer.BlockCopy(state, numSubcarriers * sizeof(double), phi, 0, numSubcarriers * sizeof(double));

                        // Calcular derivadas para cada subportadora
                        for (int i = 0; i < numSubcarriers; i++)
                        {
                            // Acoplamiento con subportadoras vecinas (difusión no local)
                            double couplingTerm = 0.0;
                            for (int j = 0; j < numSubcarriers; j++)
                            {
                                if (i != j)
                                {
                                    double distance = Math.Abs(i - j);
                                    double weight = Math.Exp(-distance * distance / 10.0);  // Decaimiento gaussiano
                                    couplingTerm += weight * (A[j] - A[i]) * CouplingStrength;
                                }
                            }

                            // Ecuación no lineal amortiguada con acoplamiento:
                            // dA/dt = -damping*A - nonlinearity*A³ + driving_force + coupling
                            deriv[i] = -DampingCoefficient * A[i]
                                     - NonlinearityCoefficient * Math.Pow(A[i], 3)
                                     + DrivingForce
                                     + couplingTerm;

                            // Evolución de fase: ω_i + nonlinear frequency shift
                            deriv[numSubcarriers + i] = frequencies[i] + A[i] * frequencies[i] * 0.5;
                        }
                    };

                    // Integración Runge-Kutta de 4to orden manual
                    var k1 = new double[numSubcarriers * 2];
                    var k2 = new double[numSubcarriers * 2];
                    var k3 = new double[numSubcarriers * 2];
                    var k4 = new double[numSubcarriers * 2];
                    var ytemp = new double[numSubcarriers * 2];

                    dydt(t, y, k1);
                    for (int i = 0; i < y.Length; i++) ytemp[i] = y[i] + k1[i] * dt / 2;
                    dydt(t + dt / 2, ytemp, k2);
                    for (int i = 0; i < y.Length; i++) ytemp[i] = y[i] + k2[i] * dt / 2;
                    dydt(t + dt / 2, ytemp, k3);
                    for (int i = 0; i < y.Length; i++) ytemp[i] = y[i] + k3[i] * dt;
                    dydt(t + dt, ytemp, k4);

                    // Actualizar y: y_new = y + (dt/6) * (k1 + 2*k2 + 2*k3 + k4)
                    for (int i = 0; i < y.Length; i++)
                    {
                        y[i] = y[i] + (dt / 6.0) * (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]);
                    }

                    // Actualizar estado desde y
                    Buffer.BlockCopy(y, 0, amplitudes, 0, numSubcarriers * sizeof(double));
                    Buffer.BlockCopy(y, numSubcarriers * sizeof(double), phases, 0, numSubcarriers * sizeof(double));
                    
                    // Constrain amplitudes al rango [0, 1]
                    for (int i = 0; i < numSubcarriers; i++)
                    {
                        amplitudes[i] = Math.Clamp(amplitudes[i], 0.0, 1.0);
                    }
                    
                    // Estadísticas del paso actual
                    double amplitudPromedio = amplitudes.Average();
                    int subportadorasActivas = amplitudes.Count(a => a > 0.2);
                    double amplitudMaxima = amplitudes.Max();
                    
                    // Calcular color del plasma basado en dinámicas actuales
                    var colorPlasma = PlasmaColor.CalcularColor(amplitudes, frequencies);
                    var colorEstado = PlasmaColor.CalcularColorEstado(countSaturacion, countDesaparicion, numSubcarriers);
                    
                    // Análisis por secciones: dividir en 4 zonas
                    int seccionSize = numSubcarriers / 4;
                    var coloresSecciones = new RgbColor[4];
                    var descSecciones = new string[4];
                    for (int sec = 0; sec < 4; sec++)
                    {
                        int inicio = sec * seccionSize;
                        int fin = (sec == 3) ? numSubcarriers : (sec + 1) * seccionSize;
                        coloresSecciones[sec] = PlasmaColor.CalcularColorSeccion(amplitudes, frequencies, inicio, fin);
                        descSecciones[sec] = PlasmaColor.DescribirColor(coloresSecciones[sec]);
                    }
                    
                    // Obtener descripciones de color
                    string descColorPlasma = PlasmaColor.DescribirColor(colorPlasma);
                    string categoriaPlasma = PlasmaColor.CategoriaPlasma(colorPlasma);
                    
                    // Detectar cambio significativo de color
                    int deltColor = Math.Abs(colorPlasma.R - colorAnterior.R) + 
                                   Math.Abs(colorPlasma.G - colorAnterior.G) + 
                                   Math.Abs(colorPlasma.B - colorAnterior.B);
                    bool colorCambio = deltColor > 40;  // Cambio significativo
                    
                    // Log logarítmico cada N pasos
                    if (paso % 20 == 0)
                    {
                        logger?.LogInformation($"[PLASMA-PASO {paso}] t={t:F2}s | Amp.Prom={amplitudPromedio:F3} | " +
                            $"Max={amplitudMaxima:F3} | Activas={subportadorasActivas}/{numSubcarriers}");
                        logger?.LogInformation($"[COLOR-GLOBAL] {colorPlasma.ToHex()} - {descColorPlasma} | {categoriaPlasma}");
                        logger?.LogInformation($"[COLOR-ESTADO] {colorEstado.ToHex()}");
                        logger?.LogInformation($"[SECCIONES] S0:{coloresSecciones[0].ToHex()}({descSecciones[0]}) | " +
                            $"S1:{coloresSecciones[1].ToHex()}({descSecciones[1]}) | " +
                            $"S2:{coloresSecciones[2].ToHex()}({descSecciones[2]}) | " +
                            $"S3:{coloresSecciones[3].ToHex()}({descSecciones[3]})");
                    }
                    
                    // Log detallado cuando hay cambio significativo de color
                    if (colorCambio)
                    {
                        logger?.LogWarning($"[TRANSICIÓN-COLOR] Paso {paso} | {colorAnterior.ToHex()} → {colorPlasma.ToHex()} | " +
                            $"De [{PlasmaColor.DescribirColor(colorAnterior)}] a [{descColorPlasma}]");
                        logger?.LogWarning($"[ETAPA-PLASMA] {categoriaPlasma}");
                    }
                    
                    for (int i = 0; i < numSubcarriers; i++)
                    {
                        var (wasSaturada, wasDesaparecida) = estadoPrevio[i];
                        bool esSaturada = amplitudes[i] >= AmplitudSaturacionThreshold;
                        bool esDesaparecida = amplitudes[i] <= AmplitudDesaparicionThreshold;

                        // Transición a saturación
                        if (esSaturada && !wasSaturada && !esDesaparecida)
                        {
                            countSaturacion++;
                            logger?.LogWarning($"[SATURACIÓN] {nombre.Texto} | Subportadora {i}: A={amplitudes[i]:F3}, φ={phases[i]:F2} | Paso {paso}");
                            await mediator.Publish(new SaturacionEvent(nombre));
                        }

                        // Transición a desaparición
                        if (esDesaparecida && !wasDesaparecida)
                        {
                            countDesaparicion++;
                            logger?.LogWarning($"[DESAPARICIÓN] {nombre.Texto} | Subportadora {i}: A={amplitudes[i]:F3}, φ={phases[i]:F2} | Paso {paso}");
                            await mediator.Publish(new DesaparicionEvent(nombre));
                        }

                        estadoPrevio[i] = (esSaturada, esDesaparecida);
                    }

                    // Normalizar fases
                    for (int i = 0; i < numSubcarriers; i++)
                    {
                        phases[i] = OscillatorSignal.NormalizePhase(phases[i]);
                    }
                    
                    // Actualizar tracking de color para próxima iteración
                    colorAnterior = colorPlasma;
                    categoriaAnterior = categoriaPlasma;

                    t += dt;
                    await Task.Delay(20);  // Simular tiempo de procesamiento
                }

                // Actualizar frame con estado final
                for (int i = 0; i < numSubcarriers; i++)
                {
                    frame[i].Amplitude = Math.Clamp(amplitudes[i], 0.0, 1.0);
                    frame[i].Phase = phases[i];
                }

                // Calcular color final del plasma
                ColorActual = PlasmaColor.CalcularColor(amplitudes, frequencies);
                var colorEstadoFinal = PlasmaColor.CalcularColorEstado(countSaturacion, countDesaparicion, numSubcarriers);
                string descColorFinal = PlasmaColor.DescribirColor(ColorActual);
                string categoriaFinal = PlasmaColor.CategoriaPlasma(ColorActual);
                
                // Análisis final por secciones
                int seccionSizeFinal = numSubcarriers / 4;
                var coloresSecconesFinal = new RgbColor[4];
                var descSecconesFinal = new string[4];
                for (int sec = 0; sec < 4; sec++)
                {
                    int inicio = sec * seccionSizeFinal;
                    int fin = (sec == 3) ? numSubcarriers : (sec + 1) * seccionSizeFinal;
                    coloresSecconesFinal[sec] = PlasmaColor.CalcularColorSeccion(amplitudes, frequencies, inicio, fin);
                    descSecconesFinal[sec] = PlasmaColor.DescribirColor(coloresSecconesFinal[sec]);
                }

                logger?.LogInformation($"[PLASMA] Simulación completada para {nombre.Texto} en {pasosSimulacion} pasos ({pasosSimulacion * dt:F1}s)");
                logger?.LogInformation($"[PLASMA-RESUMEN] Eventos generados: Saturaciones={countSaturacion}, " +
                    $"Desapariciones={countDesaparicion} | Amplitud Final Máx={amplitudes.Max():F3}, " +
                    $"Prom={amplitudes.Average():F3}");
                logger?.LogInformation($"[COLOR-FINAL-GLOBAL] {ColorActual.ToHex()} - {descColorFinal}");
                logger?.LogInformation($"[ETAPA-FINAL] {categoriaFinal}");
                logger?.LogInformation($"[SECCIONES-FINAL] S0[0-{seccionSizeFinal-1}]:{coloresSecconesFinal[0].ToHex()}({descSecconesFinal[0]}) | " +
                    $"S1[{seccionSizeFinal}-{seccionSizeFinal*2-1}]:{coloresSecconesFinal[1].ToHex()}({descSecconesFinal[1]}) | " +
                    $"S2[{seccionSizeFinal*2}-{seccionSizeFinal*3-1}]:{coloresSecconesFinal[2].ToHex()}({descSecconesFinal[2]}) | " +
                    $"S3[{seccionSizeFinal*3}-{numSubcarriers-1}]:{coloresSecconesFinal[3].ToHex()}({descSecconesFinal[3]})");

            }
            catch (Exception ex)
            {
                logger?.LogError($"[PLASMA ERROR] {nombre.Texto}: {ex.Message}. Stack: {ex.StackTrace}");
            }
        }
    }
}
