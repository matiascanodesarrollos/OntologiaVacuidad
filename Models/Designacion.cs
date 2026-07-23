using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class Designacion : Nombre
{
    public Palabra Causa { get; set; }
    public Func<double, Complex> Ventana { get; }

    internal Designacion(
        Palabra causa, 
        Func<double, Complex> ventana,
        string contexto,
        Dictionary<double, Complex> fourier)
        : base(causa.Texto, contexto, fourier, causa)
    {
        Causa = causa;
        Ventana = ventana;
    }

    /// <summary>
    /// Crea una designación dados su naturaleza, causa y un factor de atenuación exponencial.
    /// Se genera una ventana multiplicando la atenuación exponencial por la suma de los fasores de la naturaleza.
    /// </summary>
    /// <param name="naturaleza">Nombre asociado a la designación.</param>
    /// <param name="causa">Palabra que causa la designacion</param>
    /// <param name="sigma">Factor de atenuación exponencial</param>
    public Designacion(Nombre naturaleza, Palabra causa, double sigma)
        : base(naturaleza)
    {
        Causa = causa;
        Causa.Efecto = this;
        var omega = naturaleza.Fourier.Keys.Sum();
        var fase = naturaleza.Fourier.Values.Sum(v => v.Phase);
        var amplitud = naturaleza.Fourier.Values.Sum(v => v.Magnitude);
        Ventana = (t) => 
            Complex.Exp(-sigma * t)
            * Complex.FromPolarCoordinates(amplitud, omega * t + fase);
    }

    /// <summary>
    /// Obtiene la esencia de la designación, una palabra modulada por la apariencia.
    /// </summary>
    /// <param name="apariencia">Apariencia sobre la que se proyecta.</param>
    /// <param name="texto">Palabra pronunciada en texto.</param>
    /// <param name="contexto">Contexto en el que se pronuncia.</param>
    /// <returns></returns>
    public Palabra Mostrarse(
        Apariencia apariencia, 
        string texto, 
        string contexto)
    {        
        var esencia = new Palabra(
            texto: texto,
            contexto: contexto,
            frecuenciaAngular: apariencia.FrecuenciaAngular,
            admitancia: Ventana,
            Fourier)
        {
            Efecto = this, //El efecto existe antes que la causa.
        };
        return esencia;
    }

    /// <summary>
    /// Calcula la transformada de Fourier de corta duracion proyectando la designacion sobre una apariencia.
    /// Sobreescribir para definir otro criterio.
    /// Se utiliza en el metodo Aparecer de la clase Palabra, donde se multiplica por su función.
    /// </summary>
    /// <param name="apariencia">La apariencia sobre la que se proyecta la designación.</param>
    /// <param name="tau">Desplazamiento temporal de la ventana.</param>
    /// <param name="omega">Frecuencia angular de la transformada de Fourier.</param>
    /// <returns>La integral compleja.</returns>
    internal virtual Complex STFT(Apariencia apariencia, double tau, double omega)
    {
        var muestras = 100;
        var periodoMuestreo = 0.01;
        var integral = Complex.Zero;
        
        for (var n = 0; n < muestras; n++)
        {
            var t = (n - muestras / 2) * periodoMuestreo;
            var ventana = Ventana(t - tau);
            var muestra = apariencia.Funcion(t);
            var factor = Complex.FromPolarCoordinates(1.0, -omega * t);
            integral += muestra * ventana * factor;
        }

        integral *= periodoMuestreo;
        return integral;
    }
}
