using System.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace DomainLogic;

public static class Helper
{
    public static int ObtenerFrecuenciaDominante(Func<double, Complex> onda)
    {
        int n = 1024;
        var samples = new Complex[n];
        double sampleRate = 1000.0;
        for (int i = 0; i < n; i++)
        {
            double t = i / sampleRate;
            samples[i] = onda(t);
        }

        // Aplicar la Transformada Rápida de Fourier (in-place)
        Fourier.Forward(samples, FourierOptions.Default);

        double maxMagnitude = 0;
        int maxIndex = 0;

        // Analizamos solo hasta la mitad de las muestras (Teorema de Nyquist)
        for (int i = 0; i < n / 2; i++)
        {
            double magnitude = samples[i].Magnitude;
            if (magnitude > maxMagnitude)
            {
                maxMagnitude = magnitude;
                maxIndex = i;
            }
        }

        // Calcular la frecuencia real en Hz basándonos en el índice
        double frequencyResolution = (double)sampleRate / n;
        double dominantFrequency = maxIndex * frequencyResolution;

        return (int)dominantFrequency;
    }

    public static double ObtenerTiempoFinal(
        Func<double, double, Complex> onda, 
        double tiempoMaximo,
        double umbral)
    {
        for(var t = 0.0; t < tiempoMaximo; t += 1)
        {
            var valor = onda(t, 0);
            if (valor.Magnitude < umbral)
            {
                return t;
            }
        }
        return tiempoMaximo;
    }
}
