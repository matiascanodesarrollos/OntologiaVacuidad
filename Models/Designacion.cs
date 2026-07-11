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
        Ventana = (t) => 
            Complex.Exp(-sigma * t)
            * naturaleza.Fourier.Aggregate(Complex.Zero, (aggr, f) => 
                aggr 
                +   f.Value
                    * Complex.FromPolarCoordinates(1, f.Key * t)
            );
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
            Fourier);
        esencia.Funcion = (tau, t) => 
            apariencia.Fasor.Value
            * esencia.Funcion(tau, t);
        esencia.Efecto = this;
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
