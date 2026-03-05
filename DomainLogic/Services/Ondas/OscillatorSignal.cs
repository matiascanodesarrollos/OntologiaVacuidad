using System;

namespace DomainLogic.Services
{
    /// <summary>
    /// Representa una señal oscilatoria básica con fase, frecuencia y amplitud
    /// </summary>
    public class OscillatorSignal
    {
        /// <summary>
        /// Frecuencia en Hz
        /// </summary>
        public double Frequency { get; set; }

        /// <summary>
        /// Amplitud de la onda (0 a 1)
        /// </summary>
        public double Amplitude { get; set; }

        /// <summary>
        /// Fase en radianes (0 a 2π)
        /// </summary>
        public double Phase { get; set; }

        public OscillatorSignal(double frequency, double amplitude = 1.0, double phase = 0)
        {
            Frequency = frequency;
            Amplitude = amplitude;
            Phase = NormalizePhase(phase);
        }

        /// <summary>
        /// Normaliza fase al rango [0, 2π)
        /// </summary>
        public static double NormalizePhase(double phase)
        {
            return phase % (2 * Math.PI);
        }

        /// <summary>
        /// Genera valor de onda sinusoidal en el tiempo t
        /// </summary>
        public double GetValue(double time)
        {
            return Amplitude * Math.Sin(2 * Math.PI * Frequency * time + Phase);
        }

        /// <summary>
        /// Obtiene una copia de esta señal
        /// </summary>
        public OscillatorSignal Clone()
        {
            return new OscillatorSignal(Frequency, Amplitude, Phase);
        }

        public override string ToString()
        {
            return $"Signal[F:{Frequency:F2}Hz, A:{Amplitude:F3}, P:{Phase:F3}rad]";
        }
    }
}
