using System;

namespace DomainLogic.Services.Plasma
{
    public class PlasmaInteractionField
    {
        private readonly object _sync = new object();

        // Energia compartida entre vibraciones para acoplar plasmas en el tiempo.
        private double _fieldEnergy;

        public double ReadEnergy()
        {
            lock (_sync)
            {
                return _fieldEnergy;
            }
        }

        public void Update(double avgAmplitude, double maxAmplitude)
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
    }
}
