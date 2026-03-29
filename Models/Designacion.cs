using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Designacion : Apariencia
{
    public override Guid Id { get; }
    public List<Nombre> Nombres { get; }

    internal Designacion(List<string> predicados)
        : base(new Nombre(null, 0, 0, null))
    {
        Id = Guid.NewGuid();
        if(predicados == null || predicados.Count == 0)
        {            
            Nombres = new List<Nombre>();
            return;
        }

        var deltaFase = 2 * Math.PI / predicados.Count;
        var frecuenciaOraciones = predicados.Count;
        var nombres = predicados
            .Select((p, i) => new Nombre(
                p.Trim(), i * deltaFase, 
                frecuenciaOraciones - i, 
                this))
            .ToList();
        Nombres = nombres;
        Nombres.ForEach(n => n.Esencia = this);
        Causa.Esencia = this;
    }

    /// <summary>
    /// Proyeccion de un Nombre sobre una Apariencia que la modula (AM, FM, PM).
    /// </summary>
    /// <param name="nombre">El nombre proyectado.</param>
    /// <param name="apariencia">La apariencia sobre la que se proyectará el nombre.</param>
    /// <returns>La designación resultante de la proyección.</returns>
    public static Designacion Designar(Nombre nombre, Apariencia apariencia) 
    {
        return apariencia.Causa.Mostrarse(nombre);
    }

    /// <summary>
    /// Proyeccion de un Nombre sobre una Apariencia que la modula (AM, FM, PM).
    /// </summary>
    /// <param name="nombre">El nombre proyectado.</param>
    /// <param name="frecuenciaBase">La frecuencia base para la creación de la designación.</param>
    /// <returns>La designación resultante de la proyección.</returns>
    public static Designacion Designar(List<string> predicados, double frecuenciaBase) 
    {
        var aparienciaMente = new Apariencia(
            new Nombre("Ser mente luminosa", 0, frecuenciaBase, null));
        var designacion = aparienciaMente.Aparecer(new List<string> { 
            "Vacuidad",
        });

        if(predicados == null || predicados.Count == 0)
        {
            return designacion;
        }
        return Designar(designacion.Nombres.Last(), new Designacion(predicados));
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
