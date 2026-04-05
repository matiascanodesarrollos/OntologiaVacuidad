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
    /// Crea una nueva designación a partir de una lista de predicados 
    /// y una función de mapeo de texto a (fase,frecuencia,amplitud).
    /// La función de mapeo permite personalizar cómo se asignan las propiedades de cada nombre en la designación,
    /// basándose en el texto del predicado.
    /// La designación resultante tendrá una amplitud total que es la suma de las amplitudes de los nombres individuales.
    /// </summary>
    /// <param name="predicados">Los predicados que se utilizarán para crear la designación.</param>
    /// <param name="funcionMapeo">Una función opcional para mapear los predicados a sus respectivas fases, frecuencias y amplitudes. Si no se proporciona, se utilizará el mapeo predeterminado basado en la estructura de los predicados.</param>
    public static Apariencia Aparecer(List<string> predicados, 
        Func<string, (double fase, double frecuencia, double amplitud)> funcionMapeo)
    {
        var designacion = new Designacion(
            new List<Nombre>() 
            { 
                new Nombre(null, 0, 0),
            }
        );
        
        for(var i = 0; i < predicados.Count; i++)
        {
            var (fase, frecuencia, amplitud) = funcionMapeo(predicados[i]);
            var nombre = new Nombre(predicados[i], fase, frecuencia);
            nombre.Efecto.Amplitud = amplitud;
            designacion.Nombres.Add(nombre);
        }
        designacion.Amplitud = designacion.Nombres.Sum(n => n.Efecto.Amplitud);
        
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
