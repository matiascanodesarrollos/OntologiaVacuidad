using System;
using System.Linq;
using System.Numerics;

public class Designacion : Nombre
{
    public new Guid Id { get; }
    public Palabra Causa { get; set; }
    public Func<double, Complex> Ventana { get; }
    public Func<Apariencia, double, double, Complex> STFT { get; }

    /// <summary>
    /// Crea una designación dados su palabra y nombre. 
    /// Es análogo a crear una idea.
    /// </summary>
    /// <param name="naturaleza">Nombre asociado a la designación.</param>
    /// <param name="causa">Palabra que causa la designacion</param>
    /// <param name="sigma">Factor de atenuación exponencial</param>
    public Designacion(Nombre naturaleza, Palabra causa, double sigma)
        : base(naturaleza)
    {
        Id = Guid.NewGuid();
        STFT = CalcularSTFT;
        Causa = causa;
        var frecuenciaPortadora = naturaleza.Fourier.Keys.Sum();
        var amplitudPortadora = naturaleza.Fourier.ContainsKey(frecuenciaPortadora) 
                ? naturaleza.Fourier[frecuenciaPortadora] 
                : Complex.Zero;
        Ventana = (t) => 
            Complex.Exp(-sigma * t)
            * naturaleza.Fourier.Aggregate(Complex.Zero, (aggr, f) => 
                aggr 
                +   f.Value
                    * Complex.FromPolarCoordinates(1, f.Key * t)
            );
    }

    /// <summary>
    /// Calcula STFT que 
    /// </summary>
    /// <param name="omega"></param>
    /// <param name="tau"></param>
    /// <returns></returns>
    internal virtual Complex CalcularSTFT(Apariencia apariencia, double omega, double tau)
    {
        var muestras = 300;
        var periodoMuestreo = 0.01;        
        var X = Complex.Zero;

        for (int n = 0; n < muestras; n++)
        {
            var t = (n - muestras / 2) * periodoMuestreo;
            var valor = apariencia.Funcion(t);
            var w = Ventana(t - tau);
            var factor = Complex.FromPolarCoordinates(1.0, -omega * t);
            X += valor * w * factor;            
        }        

        return X;
    }

    /// <summary>
    /// Obtiene la esencia de la designación
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
            Fourier);
        esencia.Funcion = (tau, t) => 
            apariencia.Funcion(t)
            * esencia.Funcion(tau, t);
        return esencia;
    }

    /// <summary>
    /// Sobreescribe Equals para comparar designaciones por su Id.
    /// </summary>
    /// <returns>True si las designaciones son iguales, false en caso contrario.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Designacion other)
        {
            return Id == other.Id;
        }
        return false;
    }

    /// <summary>
    /// Sobreescribe GetHashCode para comparar designaciones por su Id.
    /// </summary>
    /// <returns>El hash code de la designación.</returns>
    public override int GetHashCode() => Id.GetHashCode();
}
