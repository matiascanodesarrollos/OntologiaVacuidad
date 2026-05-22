using System;
using System.Numerics;

public class Palabra : Apariencia
{
    public string Texto { get; }
    public double FrecuenciaAngular { get; }
    public Func<double, Complex> Fase { get; }
    public Nombre Nombre { get; }

    public Palabra(
        string texto, 
        double frecuenciaAngular,
        Func<double, Complex> ventana)
        : base(texto, frecuenciaAngular, ventana)
    {
        Texto = texto;
        FrecuenciaAngular = frecuenciaAngular;        
        Fase = t => Complex.FromPolarCoordinates(1.0, frecuenciaAngular * t);
        Nombre = new Nombre(Texto, CalcularVelocidadGrupo(frecuenciaAngular), ventana);
    }

    /// <summary>
    /// Calcula la velocidad de grupo para un nombre dado su frecuencia angular.
    /// Por defecto se devuelve la frecuencia en Hz.
    /// </summary>
    /// <param name="frecuenciaAngular">Frecuencia angular usada para calcular la velocidad de grupo.</param>
    /// <returns>Velocidad de grupo calculada a partir de la frecuencia angular.</returns>
    /// <remarks>
    /// Al sobreescribir este método se modifica directamente el valor usado para inicializar
    /// <see cref="Nombre"/> y, en particular, su <see cref="global::Nombre.VelocidadGrupo"/>.
    /// </remarks>
    public virtual double CalcularVelocidadGrupo(double frecuenciaAngular)
    {
        return frecuenciaAngular / (2 * Math.PI);
    }
}
