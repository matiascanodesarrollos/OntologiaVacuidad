using System;
using System.Collections.Generic;
using System.Linq;

public class Designacion
{
    public Guid Id { get; }
    public Nombre Efecto { get; }
    public (Nombre Nombre, Apariencia Apariencia) Esencia => (Efecto, new Apariencia(this));
    public Func<Nombre, double> VelocidadGrupo => n => 
        _nombres.ContainsKey(n.Frecuencia) 
            ? _nombres[n.Frecuencia].Count 
            : 0.0;

    protected Dictionary<double, List<Nombre>> _nombres { get; }
    public IEnumerable<Nombre> Nombres => _nombres.Values.SelectMany(list => list);

    internal Designacion(List<Nombre> nombres)
    {
        Id = Guid.NewGuid();
        _nombres = nombres
            .GroupBy(n => n.Frecuencia)
            .ToDictionary(g => g.Key, g => g.ToList());
        Efecto = Nombre.Cuerpo(
            _nombres.Sum(f => f.Value.Sum(n => n.Amplitud)), 
            this);
    }

    internal Designacion(string texto, Func<string, Dictionary<double, List<Nombre>>> mapeoNombres)
    {
        Id = Guid.NewGuid();
        Efecto = Nombre.Cuerpo(1.0, this);
        _nombres = mapeoNombres(texto);       
    }

    /// <summary>
    /// Crea una nueva designación al proyectar el nombre sobre la apariencia.
    /// Si la apariencia no es una designación, se toma la designación actual como apariencia.
    /// </summary>
    /// <param name="nombre">El nombre a proyectar.</param>
    /// <param name="apariencia">La apariencia sobre la cual se proyecta el nombre.</param>
    /// <returns>La nueva designación creada.</returns>
    public static Designacion Designar(Nombre nombre, Apariencia apariencia)
    {
        var designacion = apariencia as Designacion;
        var nombres = designacion.Nombres.ToList();
        nombres.Add(nombre);
        return new Designacion(nombres);
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

    /// <summary>
    /// Designación base. Vacuidad.
    /// </summary>
    public static Designacion Vacuidad = new Designacion(new List<Nombre>());
}
