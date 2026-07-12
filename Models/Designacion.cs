using System;
using System.Linq;
using System.Numerics;

public class Designacion : Nombre
{
    public new Guid Id { get; }
    public Palabra Causa { get; set; }
    public Func<double, Complex> Ventana { get; }

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
        Id = Guid.NewGuid();
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
