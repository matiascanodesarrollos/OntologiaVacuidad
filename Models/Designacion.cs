using System;
using System.Linq;
using System.Numerics;

public class Designacion : Nombre
{
    public new Guid Id { get; }
    public Palabra Causa { get; set; }
    public new Apariencia Esencia { get; set; }
    internal Func<double, double, Complex> STFT { get; }
    internal double FrecuenciaAngular { get; private set; }

    /// <summary>
    /// Crea una designación dados su palabra y nombre. 
    /// Es análogo a crear una idea.
    /// </summary>
    /// <param name="nombre">Nombre asociado a la designación.</param>
    public Designacion(Nombre nombre)
        : base(nombre)
    {
        Id = Guid.NewGuid();
        STFT = (omega, tau) => 
            nombre.Fourier(omega) 
            * Complex.FromPolarCoordinates(1.0, omega * tau);
        Esencia = nombre.Esencia;
        FrecuenciaAngular = ObtenerFrecuencia();
    }

    internal Designacion(
        Func<double, double, Complex> sTFT, 
        Nombre nombre)
        : base(nombre)
    {
        Id = Guid.NewGuid();
        STFT = sTFT;
        Esencia = nombre.Esencia;
        FrecuenciaAngular = ObtenerFrecuencia();
    }
    
    internal Designacion(Apariencia apariencia, Nombre nombre)
        : base(nombre)
    {
        Id = Guid.NewGuid();
        STFT = CalcularSTFT;
        Esencia = apariencia;
        FrecuenciaAngular = ObtenerFrecuencia();
    }

    /// <summary>
    /// Calcula la frecuencia angular de la designación a partir de su esencia, asumiendo que es una portadora pura.
    /// Se puede sobreescribir para implementar diferentes formas de cálculo de la frecuencia.
    /// </summary>
    /// <returns>La frecuencia angular calculada.</returns>
    protected virtual double ObtenerFrecuencia()
    {
        var deltaT = 0.01;
        var muestraUno = Esencia.Funcion(0.0);
        var muestraDos = Esencia.Funcion(deltaT);
        var division = muestraDos / muestraUno;
        return division.Phase / deltaT;
    }

    internal virtual Complex CalcularSTFT(double omega, double tau)
    {
        var muestras = 300;
        var periodoMuestreo = 0.01;        
        var X = Complex.Zero;

        for (int n = 0; n < muestras; n++)
        {
            var t = (n - muestras / 2) * periodoMuestreo;
            var valor = Esencia.Funcion(t);
            var w = Ventana(t - tau);
            var factor = Complex.FromPolarCoordinates(1.0, -omega * t);
            X += valor * w * factor;            
        }        

        return X;
    }

    /// <summary>
    /// Crea una apariencia para dada una palabra y se agrega a la misma como efecto. 
    /// Usa la STFT con la suma de las frecuencias (distintas) de los efectos de la palabra para crear la apariencia resultante.
    /// </summary>
    /// <param name="palabra">Palabra que dicta la onda portadora.</param>
    /// <returns>La apariencia construida.</returns>
    public Apariencia Mostrarse(Palabra palabra)
    {
        palabra.Efectos.Add(this);
        var frecuencia = palabra
            .Efectos
            .Select(e => e.FrecuenciaAngular)
            .Distinct()
            .Sum();
        var apariencia = new Apariencia(
            funcion: tau => STFT(frecuencia, tau))
        {
            Esencia = this,
        };        
        return apariencia;
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
