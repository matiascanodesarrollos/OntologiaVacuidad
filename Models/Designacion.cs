using System;
using System.Numerics;

public class Designacion : Nombre
{
    public new Guid Id { get; }
    internal double VelocidadGrupo { get; private set; }
    internal new Apariencia Esencia { get; }
    internal Func<double, double, Complex> STFT { get; }
    private Func<Complex, Complex> TransformadaZ { get; }

    /// <summary>
    /// Crea una designación dados su función de frecuencial/temporal, palabra y nombre. 
    /// Es análogo a crear una idea.
    /// </summary>
    /// <param name="sTFT">Función de Transformada de Fourier de Tiempo Corto.</param>
    /// <param name="palabra">Palabra que expresa la idea.</param>
    /// <param name="nombre">Nombre asociado a la designación.</param>
    public Designacion(
        Func<double, double, Complex> sTFT, 
        Palabra palabra, 
        Nombre nombre)
        : base(nombre)
    {
        Id = Guid.NewGuid();
        Esencia = palabra;
        STFT = sTFT;
        TransformadaZ = CalcularTransformadaZ;
    }
    
    internal Designacion(Apariencia apariencia, Nombre nombre)
        : base(nombre)
    {
        Id = Guid.NewGuid();
        Esencia = apariencia;
        STFT = CalcularSTFT;
        TransformadaZ = CalcularTransformadaZ;
    }

    /// <summary>
    /// Calcula la transformada Z evaluando la función de la esencia en la ventana del nombre 
    /// con un paso temporal de 1 por caracter del contexto.
    /// Sobreescribir para implementar diferentes formas de análisis o pasos temporales.
    /// </summary>
    /// <param name="z">Parámetro complejo de la transformada Z.</param>
    /// <returns>Valor complejo de la transformada Z en el punto z.</returns>
    internal virtual Complex CalcularTransformadaZ(Complex z)
    {        
        var muestras = 5000;
        var X = Complex.Zero;
        var periodoMuestreo = 0.01;

        for (int n = 0; n < muestras; n++)
        {
            var t = n * periodoMuestreo;
            var valor = Esencia.Funcion(t);
            var factor = Complex.Pow(z, -n);
            X += valor * factor;
        }
        
        return X;
    }

    internal virtual Complex CalcularSTFT(double omega, double tau)
    {
        var muestras = 5000;
        var periodoMuestreo = 0.01;
        var paso = 0.01;
        var X = Complex.Zero;
        var derivada = Complex.Zero;

        for (int n = 0; n < muestras; n++)
        {
            var t = n * periodoMuestreo;
            var valor = Esencia.Funcion(t);
            var w = Ventana(t - tau);
            var factor = Complex.FromPolarCoordinates(1.0, -omega * t);
            X += valor * w * factor;

            var valorPasoPositivo = Esencia.Funcion(t + paso);
            var valorPasoNegativo = Esencia.Funcion(t - paso);            
            derivada += (valorPasoPositivo - valorPasoNegativo) * factor / (2.0 * paso);
        }
        VelocidadGrupo = derivada.Magnitude > 0 ? X.Magnitude / derivada.Magnitude : 0.0;

        return X;
    }

    /// <summary>
    /// Crea una nueva palabra a partir de z y el texto deseado.
    /// Sobreescribir para implementar diferentes formas de aparición o análisis de la respuesta.
    /// </summary>    
    /// <param name="z">Valor complejo para la transformación Z.</param>
    /// <param name="texto">Texto que se desea que aparezca.</param>
    /// <returns>La nueva palabra.</returns>
    public virtual Palabra Aparecer(Complex z, string texto)
    {
        var X = TransformadaZ(z);
        var apariencia = new Apariencia(t => 
            Complex.Exp(z.Magnitude * t) 
            * X
            * Complex.FromPolarCoordinates(1, z.Phase * t))
        {
            Esencia = this,
        };

        //Causa y efecto se invierten        
        var nombre = new Nombre(
            texto,
            Texto,
            Ventana);
        var palabra = new Palabra(texto, nombre, apariencia)
        {
            Efecto = nombre, //El efecto ocurre primero que la causa (se piensa antes que la acción).            
        };
        nombre.Causa = palabra;
        return palabra;
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
