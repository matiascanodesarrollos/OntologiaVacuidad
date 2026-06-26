using System;
using System.Numerics;

public class Nombre
{
    public Guid Id { get; }
    public string Texto { get; }
    public string Contexto { get; }
    public Apariencia Esencia { get; }
    public Func<double, Complex> Ventana { get; }
    private double VelocidadGrupo { get; set; }
    private Func<Complex, Complex> TransformadaZ { get; }

    protected Nombre(Nombre otro)
    {
        Id = otro.Id;
        Texto = otro.Texto;
        Contexto = otro.Contexto;
        Ventana = otro.Ventana;
        Esencia = otro.Esencia;
        VelocidadGrupo = otro.VelocidadGrupo;
        TransformadaZ = otro.TransformadaZ;
    }

    /// <summary>
    /// Crea un nuevo nombre con texto, contexto y su transformada de Fourier.
    /// </summary>
    /// <param name="texto">Texto del nombre.</param>
    /// <param name="contexto">Contexto donde se evaluan apariciones del texto.</param>
    /// <param name="admitancia">Función de ventana: debe ponderar la referencia al nombre en cada momento del contexto.</param>
    public Nombre(string texto, 
        string contexto,
        Func<double, Complex> admitancia)
    {
        Id = Guid.NewGuid();
        Texto = texto;
        Contexto = contexto;
        Ventana = admitancia;
        Esencia = new Apariencia(
            funcion: t => admitancia(t)
        );
        TransformadaZ = CalcularTransformadaZ;
    }

    internal static Nombre Vacuidad(
        string contexto,
        double conductancia, 
        double susceptancia) => new Nombre(
            texto: nameof(Vacuidad),
            contexto: contexto,
            admitancia: t => new Complex(conductancia, susceptancia));

    /// <summary>
    /// Calcula la transformada Z evaluando la función de la esencia en la ventana del nombre 
    /// con un paso temporal de 1 por caracter del contexto.
    /// Sobreescribir para implementar diferentes formas de análisis o pasos temporales.
    /// </summary>
    /// <param name="z">Parámetro complejo de la transformada Z.</param>
    /// <returns>Valor complejo de la transformada Z en el punto z.</returns>
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
    /// Crea una nueva palabra a partir de z y el texto deseado.
    /// Sobreescribir para implementar diferentes formas de aparición o análisis de la respuesta.
    /// </summary>    
    /// <param name="z">Valor complejo para la transformación Z.</param>
    /// <param name="texto">Texto que se desea que aparezca.</param>
    /// <returns>La nueva palabra.</returns>
    public virtual Palabra Aparecer(Complex z, string texto)
    {
        var X = TransformadaZ(z);
        var apariencia = new Apariencia(
            funcion: t => 
                Complex.Exp(z.Magnitude * t) 
                * X
                * Complex.FromPolarCoordinates(1, z.Phase * t));
        var designacion = new Designacion(
            apariencia: apariencia, 
            nombre: this);
        apariencia.Esencia = designacion;
        
        var palabra = new Palabra(
            texto: texto, 
            efecto: designacion, 
            apariencia: apariencia);
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
        var paso = 0.01;
        var integral = Complex.Zero;
        var derivada = Complex.Zero;

        for (var n = 0; n < muestras; n++)
        {
            var t = (n - muestras / 2) * periodoMuestreo;
            var muestra = Ventana(t);
            var factor = Complex.FromPolarCoordinates(1.0, -omega * t);
            integral += muestra * factor;

            var valorPasoPositivo = Esencia.Funcion(t + paso);
            var valorPasoNegativo = Esencia.Funcion(t - paso);            
            derivada += (valorPasoPositivo - valorPasoNegativo) * factor / (2.0 * paso);
        }
        VelocidadGrupo = derivada.Magnitude > 0 ? integral.Magnitude / derivada.Magnitude : 0.0;

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
