using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Designacion : Apariencia
{
    public override Guid Id { get; }
    public (double X, double Y) Velocidad
    {
        get
        {
            if(Nombres.Count == 1)
            {
                return (Math.Cos(Nombres[0].Fase), Math.Sin(Nombres[0].Fase));
            }

            var nombresOrdenados = Nombres.OrderByDescending(n => n.Efecto.Amplitud).ToList();
            return (Math.Cos(nombresOrdenados[0].Fase), Math.Sin(nombresOrdenados[1].Fase));
        }
    }

    public List<Nombre> Nombres { get; internal set; }


    internal Designacion(List<Nombre> nombres)
        : base(nombres.First())
    {
        Id = Guid.NewGuid();
        Nombres = nombres;
    }

    /// <summary>
    /// Proyeccion de un Nombre sobre una Apariencia que la modula.
    /// Por defecto agrega el nombre a la lista de efectos si tiene una frecuencia menor a la maxima.
    /// </summary>
    /// <param name="nombre">El nombre proyectado.</param>
    /// <param name="apariencia">La apariencia sobre la que se proyectará el nombre.</param>
    /// <param name="funcionProyeccion">Una función opcional para personalizar la proyección del nombre sobre la apariencia.</param>
    /// <returns>La designación resultante de la proyección.</returns>
    public static Designacion Designar(Nombre nombre, Apariencia apariencia, Func<Apariencia, Nombre, Designacion> funcionProyeccion = null)
    {
        if(funcionProyeccion != null)
        {
            return funcionProyeccion(apariencia, nombre);
        }
    
        var designacion = apariencia as Designacion;
        if(designacion == null)
        {
            return new Designacion(new List<Nombre> { apariencia.Causa, nombre });
        }

        var frecuenciaMaxima = designacion
            .Nombres
            .Max(n => Math.Abs(n.Frecuencia));
        if (frecuenciaMaxima < nombre.Frecuencia)
        {
            designacion.Nombres.ForEach(n => n.Efecto.Amplitud *= 1 + nombre.Efecto.Amplitud);
            designacion.Nombres.Add(nombre);
        }
        return designacion;
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
