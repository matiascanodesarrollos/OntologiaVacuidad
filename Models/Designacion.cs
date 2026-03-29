using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Designacion : Apariencia
{
    public override Guid Id { get; }
    public List<Nombre> Nombres { get; }

    /// <summary>
    /// Crea una nueva designación a partir de una lista de predicados, donde cada predicado se convierte en un nombre con un efecto asociado a esta designación.
    /// La frecuencia de cada nombre se determina por la cantidad de nombres que comparten el mismo verbo núcleo.
    /// La amplitud se determina por la cantidad de complementos del sujeto que comparten.
    /// La fase se asigna de manera equidistante dentro del ciclo de la función de onda para los nombres que comparten la misma frecuencia, creando así una distribución uniforme en el espacio de fases.
    /// </summary>
    /// <param name="predicados"></param>
    public Designacion(List<string> predicados)
        : base(new Nombre(null, 0, 0, null))
    {
        Id = Guid.NewGuid();
        var nombres = predicados
            .Select(p => 
                new Nombre(
                    p, 
                    0, 
                    0,
                    this))
            .ToList();
        Nombres = nombres;
        Causa.Esencia = this;
        var diccionarioVerbos = predicados
            .GroupBy(p => ObtenerVerboNucleo(p))
            .ToDictionary(g => g.Key, g => g.Count());
        var diccionarioComplementos = predicados
            .SelectMany(p => ObtenerComplementos(p))
            .GroupBy(p => p)
            .ToDictionary(g => g.Key, g => g.Count());

        for(var i = 0; i < predicados.Count; i++)
        {
            Nombres[i].Efecto.Causa = Nombres[i];

            var verboNucleo = ObtenerVerboNucleo(predicados[i]);
            Nombres[i].Frecuencia = diccionarioVerbos[verboNucleo];

            var complementosDelSujeto = ObtenerComplementos(predicados[i]);
            Nombres[i].Efecto.Amplitud = Math.Max(1, complementosDelSujeto
                .Sum(c => diccionarioComplementos[c]));
        }

        var esenciasAgrupadasPorFrecuencia = Nombres.GroupBy(e => e.Frecuencia).ToList();
        foreach(var grupo in esenciasAgrupadasPorFrecuencia)
        {
            var frecuenciaGrupo = grupo.Key;
            var esenciasEnGrupo = grupo.ToList();
            var deltaFase = 2 * Math.PI / esenciasEnGrupo.Count;
            for(var i = 0; i < esenciasEnGrupo.Count; i++)
            {
                esenciasEnGrupo[i].Modular(new Palabra(null, i * deltaFase));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nombre">El nombre que se utilizará para proyectar la designación.</param>
    /// <param name="apariencia">La apariencia que se utilizará para proyectar la designación.</param>
    /// <returns>La designación resultante de la proyección.</returns>
    public static Designacion Designar(Nombre nombre, Apariencia apariencia) 
    {
        var designacion = nombre.Mostrarse(new Designacion(new List<string> { null }));
        apariencia.Modular(designacion);
        return designacion;
    }

     /// <summary>
    /// Agrega un nuevo efecto a la designacion si la frecuencia proyectada es mayor o igual a la frecuencia máxima de los efectos actuales.
    /// Se produce algo similar a la modulación FM.
    /// Sobreescribir para un comportamiento mas detallado.
    /// </summary>
    /// <param name="efectoProyectado">El nombre proyectado que se utilizará para modular la designación.</param>
    public virtual void Modular(Nombre efectoProyectado)
    {
        var frecuenciaMaxima = Nombres.Max(s => Math.Abs(s.Frecuencia));
        if(efectoProyectado.Frecuencia <= frecuenciaMaxima)
        {
            Nombres.Add(efectoProyectado);
        }
    }

    /// <summary>
    /// Sobreescribe ToString para mostrar una representación de la designación, incluyendo su naturaleza y esencia.
    /// Se muestra la naturaleza como una lista de efectos, cada uno con su causa, frecuencia y fase.
    /// </summary>
    /// <returns>Una cadena que representa la designación.</returns>
    public override string ToString()
    {
        var resultado = new StringBuilder();
        resultado.AppendLine("═══ Designación ═══");
        foreach (var efecto in Nombres)
        {
            if(efecto.Frecuencia == 0)
            {
                break;
            }
            resultado.AppendLine(efecto.ToString());
        }
        resultado.AppendLine("═══ Fin ═══");
        return resultado.ToString();
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
}
