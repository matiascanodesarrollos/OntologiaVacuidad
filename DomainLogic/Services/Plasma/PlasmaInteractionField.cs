using System;
using System.Collections.Generic;

namespace DomainLogic.Services.Plasma
{
    public class PlasmaInteractionField
    {
        private readonly object _sync = new object();

        // Energia compartida entre vibraciones para acoplar plasmas en el tiempo.
        private double _fieldEnergy;
        
        // Vectores de plasma: posición, dirección y velocidad
        private readonly Dictionary<string, VectorPlasma> _plasmaVectors = new();

        public double ReadEnergy()
        {
            lock (_sync)
            {
                return _fieldEnergy;
            }
        }

        public void Update(double maxAmplitude)
        {
            lock (_sync)
            {
                // Prioritize maxAmplitude for aggressive growth
                double incoming = maxAmplitude * 0.85;
                double proposed = (_fieldEnergy * 0.50) + (incoming * 0.50);
                // Only allow field to increase or stay the same, never decrease
                _fieldEnergy = Math.Max(_fieldEnergy, proposed);
                _fieldEnergy = Math.Clamp(_fieldEnergy, 0.0, 1.0);
            }
        }

        public void UpdatePlasmaVector(string nombreId, double posicion, double direccion, double velocidad)
        {
            lock (_sync)
            {
                _plasmaVectors[nombreId] = new VectorPlasma 
                { 
                    Posicion = posicion, 
                    Direccion = direccion, 
                    Velocidad = velocidad 
                };
            }
        }

        public VectorPlasma? ReadPlasmaVector(string nombreId)
        {
            lock (_sync)
            {
                return _plasmaVectors.TryGetValue(nombreId, out var vector) ? vector : null;
            }
        }

        public double CalculateVectorInterference(string nombreId1, string nombreId2)
        {
            lock (_sync)
            {
                if (!_plasmaVectors.TryGetValue(nombreId1, out var v1) || 
                    !_plasmaVectors.TryGetValue(nombreId2, out var v2))
                {
                    return 0.0;
                }

                // Interferencia basada en diferencia de dirección y proximidad de posición
                double direccionDiff = Math.Abs(v1.Direccion - v2.Direccion);
                // Normalizar a [0, π]
                if (direccionDiff > Math.PI)
                    direccionDiff = 2 * Math.PI - direccionDiff;

                double posicionDiff = Math.Abs(v1.Posicion - v2.Posicion);
                
                // Interferencia constructiva: direcciones similares (< 30º) o opuestas (≈180º)
                var margenDireccion = Math.PI / 6;
                bool direccionesInteractivas = (direccionDiff < margenDireccion) || 
                                               (Math.Abs(direccionDiff - Math.PI) < margenDireccion);

                if (direccionesInteractivas && posicionDiff < 0.3)
                {
                    return 1.0 - (posicionDiff / 0.3);
                }

                return 0.0;
            }
        }
    }

    public class VectorPlasma
    {
        public double Posicion { get; set; }    // Amplitud del efecto
        public double Direccion { get; set; }   // Ángulo hacia la frecuencia dominante
        public double Velocidad { get; set; }   // Velocidad de propagación
    }
}
