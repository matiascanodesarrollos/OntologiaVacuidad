using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class Nombre
{
    public Guid Id { get; }    
    public string Texto { get; }
    public string Contexto { get; }
    public Apariencia Esencia { get; }
    internal double VelocidadGrupo { get; set; }
    internal Palabra Causa { get; set; }
    internal Dictionary<double, Complex> FrecuenciasAngulares { get; }
    internal Func<double, Complex> Admitancia { get; }

    protected Nombre(Nombre otro)
    {
        Id = otro.Id;
        Texto = otro.Texto;
        Contexto = otro.Contexto;
        FrecuenciasAngulares = otro.FrecuenciasAngulares;
        Admitancia = otro.Admitancia;
        VelocidadGrupo = otro.VelocidadGrupo;
        Esencia = otro.Esencia;
        Causa = otro.Causa;
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
        Admitancia = admitancia;
        VelocidadGrupo = 0.0;
        FrecuenciasAngulares = EstimarFrecuencias();
        var frecuenciaAngular = FrecuenciasAngulares.Keys.Sum();
        Esencia = new Apariencia(frecuenciaAngular);
        Esencia.Esencia = new Designacion(Esencia, this);
    }

    internal static Nombre Vacuidad(
        string contexto,
        double componenteReal, 
        double componenteImaginario) => new Nombre(
            nameof(Vacuidad),
            contexto,
            t => new Complex(componenteReal, componenteImaginario));

    /// <summary>
    /// Crea una apariencia para este nombre de frecuencia angular igual a la suma de las frecuencias de Fourier.
    /// </summary>
    /// <param name="apariencia">Apariencia elegida para expresar el concepto.</param>
    /// <returns>La apariencia construida.</returns>
    public Apariencia Mostrarse(Apariencia apariencia)
    {
        var palabra = new Palabra(Texto, this, apariencia);
        var designacion = new Designacion(palabra, this);
        return designacion.Esencia;
    }    

    /// <summary>
    /// Calcula sigma como la transformada discreta de Fourier de la ventana sobre el contexto en la frecuencia angular especificada.
    /// Sobreescribir para definir otro criterio.
    /// </summary>
    /// <param name="omega">Frecuencia angular de análisis.</param>
    /// <returns>El integral complejo de la ventana.</returns>
    public virtual Complex Fourier(double omega)
    {
        var muestras = Math.Max(1, Contexto.Length);
        var integral = Complex.Zero;

        // Integral discreta con paso temporal unitario por caracter del contexto.
        for (var t = 0; t < muestras; t++)
        {
            var muestra = Admitancia(t);
            var factor = Complex.FromPolarCoordinates(1.0, -omega * t);
            integral += muestra * factor;
        }

        return integral;
    }

    /// <summary>
    /// Calcula la transformada de Fourier discreta completa de la ventana
    /// usando un paso temporal de 1 por carácter del contexto.
    /// </summary>
    /// <returns>Diccionario de frecuencia angular a valor complejo que representa el espectro.</returns>
    /// <remarks>
    /// Al sobreescribir este método se modifica directamente el espectro almacenado en
    /// <see cref="FrecuenciasAngulares"/> y, por tanto, las frecuencias que <see cref="Mostrarse"/> usa
    /// para construir las palabras.
    /// </remarks>
    protected virtual Dictionary<double, Complex> EstimarFrecuencias()
    {
        var totalMuestras = Math.Max(1, Contexto.Length);
        var omegas = Contexto.GroupBy(c => c + 1);
        var resultado = new Dictionary<double, Complex>();

        foreach(var grupo in omegas)
        {
            var sigma = grupo.Count();
            var omega = grupo.Key;            
            var suma = Complex.Zero;

            for (int t = 0; t < totalMuestras; t++)
            {
                var muestra = Complex.Conjugate(Admitancia(t));
                var factor = new Complex(sigma, -omega * t);
                suma += muestra * factor;
            }
            
            if(suma != Complex.Zero)
            {
                resultado.Add(omega, suma);
            }
        }

        return resultado;
    }

    /// <summary>
    /// Devuelve una representación textual simple del nombre.
    /// </summary>
    /// <returns>Una cadena con texto y velocidad de grupo.</returns>
    public override string ToString() => $"{Texto} (VelocidadGrupo: {VelocidadGrupo})";

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
