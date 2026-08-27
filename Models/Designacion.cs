using System;
using System.Numerics;

public class Designacion : Nombre
{
    public Apariencia Efecto { get; set; }
    public new Palabra Esencia { get; set; }
    public Func<double, Complex> Ventana { get; }
    public Func<double, Complex> Karma { get; }

    /// <summary>
    /// Crea una designación dados su naturaleza, efecto y un factor de atenuación exponencial.
    /// Se genera una ventana multiplicando la atenuación exponencial por la suma de los fasores de la naturaleza.
    /// </summary>
    /// <param name="naturaleza">Nombre asociado a la designación.</param>
    /// <param name="esencia">Esencia asociada a la designación.</param>
    /// <param name="ventana">Función de ventana para la designación.</param>
    /// </summary>
    public Designacion(
        Nombre naturaleza, 
        Palabra esencia, 
        Func<double, Complex> ventana)
        : base(naturaleza)
    {        
        Efecto = new Apariencia(esencia, naturaleza);
        Esencia = esencia;
        Karma = esencia.Admitancia;
        Ventana = ventana;
    }
    
    /// <summary>
    /// Aparece como una palabra a otra mente dada una frecuencia de respiración.
    /// <param name="frecuenciaRespiracion">Frecuencia de respiración de la otra mente.</param>
    /// </summary>
    public Palabra Aparecer(double frecuenciaRespiracion)
    {
        var palabra = new Palabra(
            Texto,
            Esencia.FrecuenciaAngular + frecuenciaRespiracion,
            t => Ventana(t) * Karma(t)
        );
        return palabra;
    }

    /// <summary>
    /// Calcula la admitancia de la designación a una frecuencia angular, un tiempo y una frecuencia angular prima.
    /// Se usa para obtener la admitancia de la designación.
    /// Sobreescribir para definir otro criterio.
    /// </summary>
    /// <param name="omega">Frecuencia angular de análisis.</param>
    /// <param name="tau">Tiempo de análisis.</param>
    /// <param name="omegaPrima">Frecuencia angular prima de análisis.</param>
    /// <returns>La admitancia compleja de la designación.</returns>
    public virtual Complex CalcularAdmitancia(double omega, double tau, double omegaPrima)
    {
        var muestras = 100;
        var periodoMuestreo = 0.01;
        var integral = Complex.Zero;
        
        for (var n = 0; n < muestras; n++)
        {
            var t = (n - muestras / 2) * periodoMuestreo;
            var muestra = Efecto.Funcion(t) * Ventana(t - tau) * Karma(t - tau);
            var frecuencia = Complex.FromPolarCoordinates(1.0, -omega * t);
            var effectoDoppler = Complex.FromPolarCoordinates(1.0, omegaPrima * t);
            integral += muestra * frecuencia * effectoDoppler;
        }

        integral *= periodoMuestreo;
        return integral;
    }
}
