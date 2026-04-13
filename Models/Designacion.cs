using System;
using System.Collections.Generic;
using System.Linq;

public class Designacion : Apariencia
{
    public override Guid Id { get; }
    internal List<Nombre> _nombres { get; set; }
    public IEnumerable<Nombre> Nombres => _nombres.AsReadOnly();

    /// <summary>
    /// Función que determina el efecto dado un tiempo y una frecuencia.
    /// </summary>
    public Func<(double Tiempo, double Frecuencia), (double Amplitud, double Fase)> Efectos { get; }

    /// <summary>
    /// Función que determina la velocidad de grupo dada una frecuencia.
    /// </summary>
    public Func<double, double> VelocidadGrupo { get; }

    internal Designacion(
        List<Nombre> nombres,
        Func<(double Tiempo, double Frecuencia), (double Amplitud, double Fase)> efectos,
        Func<double, double> velocidadGrupo)
        : base(nombres.First().Esencia.Amplitud)
    {
        Id = Guid.NewGuid();
        _nombres = nombres;
        Efectos = efectos;
        VelocidadGrupo = velocidadGrupo;
    }

    /// <summary>
    /// Crea una nueva designación al proyectar el nombre sobre la apariencia, al mostrarse con una ventana Gaussiana.
    /// </summary>
    /// <param name="apariencia">La apariencia sobre la cual proyectar el nombre.</param>
    /// <param name="nombre">El nombre a proyectar.</param>
    /// <param name="ventana">La función un mapeo de tiempo a frecuencia.</param>
    /// <returns>La nueva designación creada.</returns>
    public Designacion Designar(Apariencia apariencia, 
        Nombre nombre,
        Func<double, double> ventana)
    {
        Func<(double Tiempo, double Frecuencia), 
            (double Amplitud, double Fase)> efectos = x =>
        {
            var amplitud = apariencia.Amplitud 
                    * Math.Cos(x.Frecuencia * x.Tiempo + nombre.Fase)
                + nombre.Amplitud(ventana(x.Tiempo))
                    * Math.Sin(x.Frecuencia * x.Tiempo + nombre.Fase);
            var fase = x.Tiempo % (2 * Math.PI);
            return (amplitud, fase);
        };

        var designacion = apariencia as Designacion;
        var nombres = new List<Nombre>(designacion.Nombres)
        {
            nombre,
        };
        var nuevaDesignacion = new Designacion(nombres, efectos, designacion.VelocidadGrupo);
        return nuevaDesignacion;
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
