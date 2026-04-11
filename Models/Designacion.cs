using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Designacion : Apariencia
{
    public override Guid Id { get; }
    /// <summary>
    /// Función que determina la velocidad de grupo dada una frecuencia.
    /// </summary>
    public Func<double, double> VelocidadGrupo { get; internal set; }

    internal List<Nombre> _nombres { get; set; }
    public IEnumerable<Nombre> Nombres => _nombres.AsReadOnly();

    public Designacion(List<Nombre> nombres)
        : base(nombres.First(), 1.0)
    {
        Id = Guid.NewGuid();
        _nombres = nombres;
        VelocidadGrupo = frecuencia => 1.0;
    }

    /// <summary>
    /// Crea una nueva designación al proyectar el nombre sobre la apariencia, al mostrarse con una ventana Gaussiana.
    /// </summary>
    /// <param name="apariencia">La apariencia sobre la cual proyectar el nombre.</param>
    /// <param name="nombre">El nombre a proyectar.</param>
    /// <param name="ventana">La función que define la ventana Gaussiana para la proyección.</param>
    /// <returns>La nueva designación creada.</returns>
    public Designacion Designar(Apariencia apariencia, 
        Nombre nombre,
        Func<Nombre, bool> ventana = null)
    {
        var nuevaDesignacion = new Designacion(new List<Nombre> { nombre });

        var designacion = apariencia as Designacion;
        if (designacion == null)
        {
            return nuevaDesignacion;
        }

        designacion._nombres.Add(nombre);

        if(ventana == null)
        {
            // Similar a Gaussiana
            ventana = n => nombre
                .Efecto
                .Keys
                .Any(f => f >= n.Efecto.Keys.Min() && f <= n.Efecto.Keys.Max());
        }
        nuevaDesignacion
            ._nombres
            .AddRange(designacion.Nombres.Where(n => ventana(n)));
        
        return nuevaDesignacion;
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
        foreach (var nombre in Nombres)
        {
            resultado.AppendLine(nombre.ToString());
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
