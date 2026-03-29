using System;
using System.Collections.Generic;
using System.Linq;

public class Apariencia
{
    public virtual Guid Id { get; }
    public double Amplitud { get; internal set; }
    public Nombre Causa { get; internal set; }

    internal Apariencia(Nombre causa)
    {
        Id = Guid.NewGuid();
        Amplitud = 1.0;
        Causa = causa;
    }

    /// <summary>
    /// Modula la amplitud de la apariencia basada en el efecto del nombre proyectado.
    /// Se produce algo similar a la modulación AM, donde la amplitud de la apariencia se ajusta en función del efecto del nombre proyectado.
    /// Sobreescribir para un comportamiento mas detallado.
    /// </summary>
    /// <param name="designacionProyectada">La designación que se utilizará para modular la apariencia.</param>
    public virtual void Modular(Designacion designacionProyectada)
    {
        Amplitud *= 1 + designacionProyectada.Nombres.Sum(n => n.Efecto.Amplitud);
    }

    /// <summary>
    /// Crea una nueva designación a partir de una lista de predicados, donde cada predicado se convierte en un nombre con un efecto asociado a esta designación.
    /// La frecuencia de cada nombre se determina por la cantidad de nombres que comparten el mismo verbo núcleo.
    /// La amplitud se determina por la cantidad de complementos del sujeto que comparten.
    /// La fase se asigna de manera equidistante dentro del ciclo de la función de onda para los nombres que comparten la misma frecuencia, creando así una distribución uniforme en el espacio de fases.
    /// </summary>
    /// <param name="predicados">Los predicados que se utilizarán para crear la designación.</param>
    public Designacion Aparecer(List<string> predicados)
    {
        var designacion = new Designacion(predicados);
        Causa = designacion.Nombres.FirstOrDefault();
        return designacion;
    }
}
