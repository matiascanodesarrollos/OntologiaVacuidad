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
    /// Crea una nueva designación a partir de una lista de predicados, donde cada predicado se convierte en un nombre con un efecto asociado a esta designación.
    /// Por defecto, la frecuencia de cada nombre se determina por la cantidad de nombres que comparten el mismo verbo núcleo.
    /// La amplitud se determina por la cantidad de complementos del sujeto que comparten.
    /// La fase se asigna de manera equidistante dentro del ciclo de la función de onda para los nombres que comparten la misma frecuencia, creando así una distribución uniforme en el espacio de fases.
    /// Se puede pasar otra funcion de mapeo.
    /// </summary>
    /// <param name="predicados">Los predicados que se utilizarán para crear la designación.</param>
    /// <param name="funcionMapeo">Una función opcional para mapear los predicados a sus respectivas fases, frecuencias y amplitudes. Si no se proporciona, se utilizará el mapeo predeterminado basado en la estructura de los predicados.</param>
    public static Apariencia Aparecer(List<string> predicados, 
        Func<List<string>, List<(double fase, double frecuencia, double amplitud)>> funcionMapeo = null)
    {
        var designacion = new Designacion(
            new List<Nombre>() 
            { 
                new Nombre(null, 0, 0)
            } 
        );
        
        if(funcionMapeo != null)
        {
            var parametros = funcionMapeo(predicados);
            for(var i = 0; i < predicados.Count; i++)
            {
                var (fase, frecuencia, amplitud) = parametros[i];
                var nombre = new Nombre(predicados[i].Trim(), fase, frecuencia);
                designacion.Nombres.Add(nombre);
            }

            return designacion;
        }
        
        var deltaFasePredicados = 2 * Math.PI / predicados.Count;
        var frecuenciaOraciones = predicados.Count;
        designacion.Nombres.AddRange(predicados
            .Select((p, i) => new Nombre(
                p.Trim(), 
                i * deltaFasePredicados, 
                frecuenciaOraciones - i))
            .ToList());

        var diccionarioVerbos = predicados
            .GroupBy(p => p.Split(' ').First())
            .ToDictionary(g => g.Key, g => g.Count());
        var diccionarioComplementos = predicados
            .SelectMany(p => p.Split(' ').Skip(1))
            .GroupBy(p => p)
            .ToDictionary(g => g.Key, g => Math.Max(1,g.Count()));

        for(var i = 0; i < predicados.Count; i++)
        {
            var verboNucleo = predicados[i].Split(' ').First();
            designacion.Nombres[i].Frecuencia *= diccionarioVerbos[verboNucleo];

            var complementosDelSujeto = predicados[i].Split(' ').Skip(1).ToList();
            designacion.Nombres[i].Efecto.Amplitud = complementosDelSujeto
                .Sum(c => diccionarioComplementos[c]);
        }
        
        return designacion;
    }

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
