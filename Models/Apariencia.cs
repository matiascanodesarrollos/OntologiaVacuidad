using System;
using System.Collections.Generic;
using System.Linq;

public class Apariencia
{
    public virtual Guid Id { get; }
    public double Amplitud { get; internal set; }

    internal Apariencia(double amplitud)
    {
        Id = Guid.NewGuid();
        Amplitud = amplitud;
    }

    /// <summary>
    /// Crea una nueva designación a partir de una lista de predicados y una función de mapeo de texto a (fase,frecuencia,amplitud).
    /// La misma permite personalizar cómo se asignan las propiedades de cada nombre en la designación,
    /// basándose en el texto del predicado.
    /// La designación resultante tendrá una amplitud total que es la suma de las amplitudes de los nombres individuales.
    /// </summary>
    /// <param name="predicados">Los predicados que se utilizarán para crear la designación.</param>
    /// <param name="funcionMapeoPredicadoValor">Una función para mapear los predicados a sus respectivas fases y amplitudes. </param>
    /// <param name="funcionMapeoFrecuenciaVelocidad">Una función para mapear las frecuencias a sus respectivas velocidades. </param>
    /// <returns>Una nueva designación creada a partir de los predicados y las funciones de mapeo.</returns>
    public static Apariencia Aparecer(List<string> predicados, 
        Func<string, (double fase, double amplitud)> funcionMapeoPredicadoValor,
        Func<double, double> funcionMapeoFrecuenciaVelocidad)
    {
        var nombres = new List<Nombre>();
        var velocidades = new Dictionary<double, double>();
        for(var i = 0; i < predicados.Count; i++)
        {
            var (fase, amplitud) = funcionMapeoPredicadoValor(predicados[i]);
            var nombre = new Nombre(predicados[i], fase, f => amplitud);
            nombres.Add(nombre);
        }

        Func<(double Tiempo, double Frecuencia), (double Amplitud, double Fase)> efectos = 
        x =>
        {
            var tiempo = x.Tiempo % (2 * Math.PI);
            var faseObjetivo = Math.Abs(tiempo);
            var amplitud = nombres
                .OrderBy(a => Math.Abs(a.Fase - faseObjetivo))
                .Select(a => a.Esencia.Amplitud)
                .FirstOrDefault();
            return (amplitud, tiempo);
        };
        var designacion = new Designacion(nombres, efectos, funcionMapeoFrecuenciaVelocidad);
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
