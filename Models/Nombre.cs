using System;
using System.Numerics;

public class Nombre
{
    public Guid Id { get; }
    public string Texto { get; }
    public string Contexto { get; }
    public Apariencia Esencia { get; internal set; }
    public Func<double, Complex> Ventana { get; }

    protected Nombre(Nombre otro)
    {
        Id = otro.Id;
        Texto = otro.Texto;
        Contexto = otro.Contexto;
        Ventana = otro.Ventana;
        Esencia = otro.Esencia;
    }

    internal Nombre(string texto, 
        string contexto,
        Func<double, Complex> admitancia,
        Apariencia esencia)
    {
        Id = Guid.NewGuid();
        Texto = texto;
        Contexto = contexto;
        Ventana = admitancia;
        Esencia = esencia;
    }

    internal static Nombre Vacuidad(
        string contexto) => new Nombre(
            texto: nameof(Vacuidad),
            contexto: contexto,
            admitancia: t => Complex.Zero,
            esencia: new Apariencia(
                funcion: t => Complex.Zero)
        );

    /// <summary>
    /// Crea una nueva palabra a partir de s y el texto deseado.
    /// Sobreescribir para implementar diferentes formas de aparición o análisis de la respuesta.
    /// </summary>    
    /// <param name="s">Valor complejo para la transformación de Laplace.</param>
    /// <param name="textoPalabra">Texto que se desea que aparezca.</param>
    /// <returns>La nueva palabra.</returns>
    public virtual Palabra Aparecer(Complex s, string textoPalabra)
    {
        var X = CalcularLaplace(s);
        var periodoMuestreo = 0.01;
        var z = Complex.Exp(s * periodoMuestreo);
        var H = Esencia.Esencia?.CalcularTransformadaZ(z) ?? Complex.One;
        var Y = H * X;

        var apariencia = new Apariencia(
            funcion: t => Y * Complex.FromPolarCoordinates(1, s.Phase * t));
        var designacion = new Designacion(
            apariencia: apariencia, 
            nombre: this);
        apariencia.Esencia = designacion;
        var palabra = new Palabra(
            texto: textoPalabra, 
            efecto: designacion, 
            apariencia: apariencia)
        {
            Esencia = designacion,
        };
        designacion.Causa = palabra;
        return palabra;
    }

    /// <summary>
    /// Calcula la transformada de Fourier de la ventana.
    /// Sobreescribir para definir otro criterio.
    /// </summary>
    /// <param name="omega">Frecuencia angular de análisis.</param>
    /// <returns>El integral complejo de la ventana.</returns>
    public virtual Complex Fourier(double omega)
    {
        var muestras = 100;
        var periodoMuestreo = 0.01;
        var integral = Complex.Zero;

        for (var n = 0; n < muestras; n++)
        {
            var t = (n - muestras / 2) * periodoMuestreo;
            var muestra = Ventana(t);
            var factor = Complex.FromPolarCoordinates(1.0, -omega * t);
            integral += muestra * factor;
        }

        return integral;
    }

    internal virtual Complex CalcularLaplace(Complex s)
    {
        var muestras = 100;
        var periodoMuestreo = 0.01;
        var integral = Complex.Zero;

        for (var n = 0; n < muestras; n++)
        {
            var t = (n - muestras / 2) * periodoMuestreo;
            var muestra = Ventana(t);
            var factor = Complex.Exp(-s * t);
            integral += muestra * factor;
        }

        return integral;
    }

    /// <summary>
    /// Devuelve una representación textual simple del nombre.
    /// </summary>
    /// <returns>Una cadena con texto y velocidad de grupo.</returns>
    public override string ToString() => $"{Texto}";

    /// <summary>
    /// Compara nombres por su Id.
    /// </summary>
    /// <returns>True si ambos nombres tienen el mismo Id, false en caso contrario.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Nombre other)
        {
            return Id == other.Id;
        }
        return false;
    }

    /// <summary>
    /// Genera el hash code a partir del Id.
    /// </summary>
    /// <returns>El hash code del Id del nombre.</returns>
    public override int GetHashCode() => Id.GetHashCode();
}
