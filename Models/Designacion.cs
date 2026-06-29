using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class Designacion : Nombre
{
    public new Guid Id { get; }
    public Palabra Causa { get; set; }
    public new Palabra Esencia { get; set; }
    private Func<Complex, Complex> TransformadaZ { get; }
    public Func<double, double, Complex> STFT { get; }
    internal double FrecuenciaAngular { get; private set; }

    /// <summary>
    /// Crea una designación dados su palabra y nombre. 
    /// Es análogo a crear una idea.
    /// </summary>
    /// <param name="texto">Texto de la designación.</param>
    /// <param name="contexto">Contexto de la designación.</param>
    /// <param name="frecuenciaAngularRespiracion">Frecuencia angular de respiración durante la designación.</param>
    /// <param name="transformadaZ">Transformada Z de la designación.</param>
    public Designacion(string texto, 
        string contexto,
        double frecuenciaAngularRespiracion,
        Dictionary<Complex, Complex> transformadaZ)
        : base(
            texto: texto, 
            contexto: contexto, 
            admitancia: t => transformadaZ.Aggregate(Complex.Zero, (acc, x) => 
                acc + x.Value 
                * Complex.Exp(x.Key.Magnitude * t)
                * Complex.FromPolarCoordinates(1, x.Key.Phase * t)), 
            esencia: new Apariencia(
                t => transformadaZ[new Complex(1, frecuenciaAngularRespiracion)] 
                    * Complex.FromPolarCoordinates(1, frecuenciaAngularRespiracion * t)
                )
        )
    {
        Id = Guid.NewGuid();
        TransformadaZ = z => transformadaZ.ContainsKey(z) ? transformadaZ[z] : Complex.Zero;
        STFT = (omega, tau) =>
        {
            var z = Complex.Exp(new Complex(1, omega));
            var H = TransformadaZ(z);
            return H * Complex.FromPolarCoordinates(1, omega * tau);
        };
        FrecuenciaAngular = frecuenciaAngularRespiracion;
        Esencia = new Palabra(
            texto: Texto,
            funcion: (tau, t) => 
                Complex.FromPolarCoordinates(1.0, frecuenciaAngularRespiracion * tau)
                * Ventana(t - tau),
            funcionApariencia: t => 
                Fourier(frecuenciaAngularRespiracion)
                * Complex.FromPolarCoordinates(1.0, frecuenciaAngularRespiracion * t)
            )
        {
            Esencia = this,
            Efectos = { this },
        };
    }
    
    internal Designacion(Apariencia apariencia, Nombre nombre)
        : base(nombre)
    {
        Id = Guid.NewGuid();
        STFT = CalcularSTFT;
        var deltaT = 0.01;
        var muestraUno = apariencia.Funcion(0);
        var muestraDos = apariencia.Funcion(deltaT);
        var division = muestraDos / muestraUno;
        FrecuenciaAngular = division.Phase / deltaT;
        Esencia = new Palabra(
            texto: nombre.Texto,
            efecto: this,
            apariencia: apariencia);        
    }

    internal static Designacion Gozo(double energia, string nombre) 
    {
        var frecuenciaAngular = double.Epsilon;
        var valor = new Complex(energia, 0.0);
        var palabra = new Palabra(
            texto: nombre,
            funcion: (tau, t) => valor,
            funcionApariencia: t => Complex.FromPolarCoordinates(energia, frecuenciaAngular * t)
        );
        var designacion = new Designacion(
            texto: nombre,
            contexto: nameof(Vacuidad),
            frecuenciaAngularRespiracion: double.Epsilon,
            transformadaZ: new Dictionary<Complex, Complex>()
            {
                { Complex.FromPolarCoordinates(1, frecuenciaAngular), Complex.Zero },
                { Complex.FromPolarCoordinates(1, Math.PI / 4), valor },
                { Complex.FromPolarCoordinates(1, 3 * Math.PI / 4), valor },
                { Complex.FromPolarCoordinates(1, 5 * Math.PI / 4), valor },
                { Complex.FromPolarCoordinates(1, 7 * Math.PI / 4), valor },
            }
        );
        
        palabra.Esencia = designacion;
        palabra.Efectos.Add(designacion);
        return designacion;
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
        var apariencia = new Apariencia(frecuencia)
        {
            Esencia = this,
        };        
        return apariencia;
    }

    /// <summary>
    /// Calcula la STFT de la designación para una frecuencia angular y un tiempo de retardo dados.
    /// </summary>
    /// <param name="omega">Frecuencia angular de análisis.</param>
    /// <param name="tau">Tiempo de retardo.</param>
    /// <returns>El valor complejo de la STFT en el punto dado.</returns>
    internal virtual Complex CalcularSTFT(double omega, double tau)
    {
        var muestras = 300;
        var periodoMuestreo = 0.01;        
        var X = Complex.Zero;
        var apariencia = Esencia as Apariencia;

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
    
    internal virtual Complex CalcularTransformadaZ(Complex z)
    {        
        var muestras = 300;
        var X = Complex.Zero;
        var periodoMuestreo = 0.01;

        for (int n = 0; n < muestras; n++)
        {
            var t = (n - muestras / 2) * periodoMuestreo;
            var valor = Ventana(t);
            var factor = Complex.Pow(z, -n);
            X += valor * factor;
        }
        
        return X;
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
