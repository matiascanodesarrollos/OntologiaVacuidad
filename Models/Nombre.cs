using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class Nombre
{
    public string Texto { get; }
    private readonly Lazy<Dictionary<double, Complex>> _fourier;
    public Dictionary<double, Complex> Fourier => _fourier.Value;
    public Func<double, Complex> Ventana { get; }
    public double VelocidadGrupo { get; }

    /// <summary>
    /// Crea un nuevo nombre con texto, velocidad de grupo y su transformada de Fourier.
    /// </summary>
    /// <param name="texto">Texto del nombre.</param>
    /// <param name="velocidadGrupo">Velocidad de propagación asociada al nombre.</param>
    /// <param name="ventana">Función de ventana que devuelve un valor complejo para un tiempo dado.</param>
    public Nombre(string texto, 
        double velocidadGrupo,
        Func<double, Complex> ventana)
    {
        Texto = texto;
        VelocidadGrupo = velocidadGrupo;
        Ventana = ventana;
        _fourier = new Lazy<Dictionary<double, Complex>>(CalcularTransformadaFourier);
    }

    /// <summary>
    /// Proyecta esta designación en una nueva apariencia evaluando su función en una frecuencia angular dada.
    /// </summary>
    /// <returns>Una apariencia construida a partir de la función de esta designación.</returns>
    public Apariencia Mostrarse()
    {        
        var mente = new Designacion(Apariencia.Mente(Fourier.Count), this);
        var palabras = new List<Palabra>()
        {
            new Palabra(Texto, mente.FrecuenciaAngular, Ventana, VelocidadGrupo),            
        };
        palabras.AddRange(Fourier.Select((kv, i) => new Palabra(
            $"{Texto}_{i}",
            kv.Key,
            t => 1,
            VelocidadGrupo
        )));
        return Apariencia.Aparecer(palabras, VelocidadGrupo);
    }

    /// <summary>
    /// Devuelve una representación textual simple del nombre.
    /// </summary>
    /// <returns>Una cadena con texto y velocidad de grupo.</returns>
    public override string ToString() => $"{Texto} (VelocidadGrupo: {VelocidadGrupo})";

    /// <summary>
    /// Compara nombres por su texto.
    /// </summary>
    /// <returns>True si ambos nombres tienen el mismo texto, false en caso contrario.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Nombre other)
        {
            return Texto == other.Texto;
        }
        return false;
    }

    /// <summary>
    /// Genera el hash code a partir del texto.
    /// </summary>
    /// <returns>El hash code del texto del nombre.</returns>
    public override int GetHashCode() => Texto.GetHashCode();

    /// <summary>
    /// Calcula la transformada de Fourier de la ventana sobre un rango de frecuencias angulares.
    /// </summary>
    /// <returns>Diccionario de frecuencia angular a valor complejo que representa el espectro.</returns>
    /// <remarks>
    /// Al sobreescribir este método se modifica directamente el espectro almacenado en
    /// <see cref="Fourier"/> y, por tanto, las frecuencias que <see cref="Mostrarse"/> usa
    /// para construir las palabras. La implementación derivada debería conservar
    /// estabilidad numérica y devolver valores finitos para evitar propagar NaN o infinito.
    /// </remarks>
    protected virtual Dictionary<double, Complex> CalcularTransformadaFourier()
    {
        const double limiteIntegracion = 8.0;
        const int pasos = 512;
        const int numFreqs = 64;
        double dt = 2.0 * limiteIntegracion / pasos;
        double frecuenciaMax = Math.PI * numFreqs / (2.0 * limiteIntegracion);
        double dw = 2.0 * frecuenciaMax / numFreqs;

        var resultado = new (double FrecuenciaAngular, Complex Valor)[numFreqs];

        for (int k = 0; k < numFreqs; k++)
        {
            double omega = -frecuenciaMax + k * dw;
            var suma = Complex.Zero;
            for (int i = 0; i <= pasos; i++)
            {
                double t = -limiteIntegracion + i * dt;
                double peso = (i == 0 || i == pasos) ? 0.5 : 1.0;
                suma += peso * Ventana(t) * Complex.FromPolarCoordinates(1.0, -omega * t);
            }
            resultado[k] = (omega, suma * dt);
        }

        return resultado.ToDictionary(p => p.FrecuenciaAngular, p => p.Valor);
    }    
}
