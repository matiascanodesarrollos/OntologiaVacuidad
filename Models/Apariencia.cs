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
    public Designacion Aparecer(List<string> predicados = null)
    {
        if(predicados == null)
        {
            predicados = new List<string> {
                "Apariencia",
                "Aparecer tierra pura",
                "Aparecer agua vibración pura y sólida",
                "Aparecer aire vibración pura",
                "Aparecer fuego",
            };
        }

        var designacion = new Designacion(predicados);        
        Causa = designacion.Nombres.First();

        var diccionarioVerbos = predicados
            .GroupBy(p => ObtenerVerboNucleo(p))
            .ToDictionary(g => g.Key, g => g.Count());
        var diccionarioComplementos = predicados
            .SelectMany(p => ObtenerComplementos(p))
            .GroupBy(p => p)
            .ToDictionary(g => g.Key, g => g.Count());

        for(var i = 0; i < predicados.Count; i++)
        {
            designacion.Nombres[i].Efecto.Causa = designacion.Nombres[i];

            var verboNucleo = ObtenerVerboNucleo(predicados[i]);
            designacion.Nombres[i].Frecuencia = Causa.Frecuencia * diccionarioVerbos[verboNucleo];

            var complementosDelSujeto = ObtenerComplementos(predicados[i]);
            designacion.Nombres[i].Efecto.Amplitud = Math.Max(1, complementosDelSujeto
                .Sum(c => diccionarioComplementos[c]));
        }

        var esenciasAgrupadasPorFrecuencia = designacion.Nombres.GroupBy(e => e.Frecuencia).ToList();
        foreach(var grupo in esenciasAgrupadasPorFrecuencia)
        {
            var frecuenciaGrupo = grupo.Key;
            var esenciasEnGrupo = grupo.ToList();
            var deltaFase = 2 * Math.PI / esenciasEnGrupo.Count;
            for(var i = 0; i < esenciasEnGrupo.Count; i++)
            {
                esenciasEnGrupo[i].Fase = i * deltaFase;
            }
        }
        return designacion;
    }

    /// <summary>
    /// Asume que el verbo nucleo es la primera palabra del predicado.
    /// Se usa en el contructor para determinar la frecuencia de cada nombre en la designación.
    /// Sobreescribir para un comportamiento mas detallado.
    /// </summary>
    /// <param name="predicado">El predicado del cual se extraerá el verbo nucleo.</param>
    /// <returns>El verbo nucleo de un predicado.</returns>
    public virtual Func<string, string> ObtenerVerboNucleo => predicado => predicado
        .Split(' ')
        .First();

    /// <summary>
    /// Asume que los complementos son todas las palabras del predicado excepto la primera.
    /// Se usa en el contructor para determinar la amplitud de cada nombre en la designación.
    /// Sobreescribir para un comportamiento mas detallado.
    /// </summary>
    /// <param name="predicado">El predicado del cual se extraerán los complementos.</param>
    /// <returns>La lista de complementos de un predicado.</returns>
    public virtual Func<string, List<string>> ObtenerComplementos => predicado => predicado
        .Split(' ')
        .Where((_, index) => index > 0)
        .ToList();

    /// <summary>
    /// Sobreescribe GetHashCode para comparar apariencias por su Id.
    /// </summary>
    /// <returns>El hash code de la apariencia.</returns>
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    /// Sobreescribe Equals para comparar apariencias por su Id.
    /// </summary>
    /// <returns>True si las apariencias son iguales, false en caso contrario.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Apariencia other)
        {
            return Id == other.Id;
        }
        return false;
    }
}
