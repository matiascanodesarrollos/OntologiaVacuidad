using System;
using System.Numerics;

public class Apariencia : Palabra
{ 
    public Func<double, Complex> Funcion { get; }
    public Complex Fasor { get; }
    public Nombre Esencia { get; set; }
    public Designacion Causa { get; set; }

    /// <summary>
    /// Crea a partir de una palabra y el nombre de su esencia.
    /// El fasor se calcula como la transformada de Fourier de la admitancia de la palabra.
    /// La función de la apariencia es el fasor multiplicado por la exponencial compleja.
    /// <param name="naturaleza">Palabra de la que se deriva la apariencia.</param>
    /// <param name="esencia">Nombre de la esencia de la apariencia.</param>
    /// </summary>
    public Apariencia(Palabra naturaleza, Nombre esencia) 
        : base(naturaleza)
    {
        Esencia = esencia;
        Fasor = CalcularFourier(FrecuenciaAngular);
        Funcion = t => 
            Fasor 
            * Complex.FromPolarCoordinates(1, FrecuenciaAngular * t);
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
        var palabra = this as Palabra;
        
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

