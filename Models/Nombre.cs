using System.Collections.Generic;
using System.Numerics;

public class Nombre
{
    public string Texto { get; }
    public string Contexto { get; }
    public Apariencia Esencia { get; }
    internal Dictionary<double, Complex> Fourier { get; }

    protected Nombre(Nombre otro)
    {
        Texto = otro.Texto;
        Contexto = otro.Contexto;        
        Esencia = otro.Esencia;
        Fourier = otro.Fourier;
    }

    /// <summary>
    /// Crea un nuevo nombre con texto, contexto y su transformada de Fourier.
    /// </summary>
    /// <param name="texto">Texto del nombre.</param>
    /// <param name="contexto">Contexto donde se evaluan apariciones del texto.</param>
    public Nombre(string texto, 
        string contexto,
        Dictionary<double, Complex> fourier,
        Apariencia esencia)
    {
        Texto = texto;
        Contexto = contexto;
        Fourier = fourier;
        Esencia = esencia;
    }    

    /// <summary>
    /// Calcula la transformada de Fourier de la admitancia de la palabra.
    /// Se usa para obtener el fasor de la apariencia.
    /// Sobreescribir para definir otro criterio.
    /// </summary>
    /// <param name="omega">Frecuencia angular de análisis.</param>
    /// <returns>El integral complejo de la ventana.</returns>
    public virtual Complex CalcularFourier(double omega)
    {
        var muestras = 100;
        var periodoMuestreo = 0.01;
        var integral = Complex.Zero;
        var palabra = Esencia as Palabra;
        
        for (var n = 0; n < muestras; n++)
        {
            var t = (n - muestras / 2) * periodoMuestreo;
            var muestra = palabra.Admitancia(t);
            var factor = Complex.FromPolarCoordinates(1.0, -omega * t);
            integral += muestra * factor;
        }

        integral *= periodoMuestreo;
        return integral;
    }

}
