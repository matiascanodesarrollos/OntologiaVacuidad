using System;
using System.Collections.Generic;
using System.Linq;

public class Designacion
{
    public Guid Id { get; }
    public (Nombre Nombre, Apariencia Apariencia) Efecto => (
        new Nombre(
            "Vacuidad",
            0,
            0,
            _nombres.Sum(f => f.Value.Sum(n => n.Amplitud)), 
            this),
        new Apariencia(this));
    /// <summary>
    /// Función que determina la velocidad de grupo dada un nombre.
    /// </summary>
    public Func<Nombre, double> VelocidadGrupo => n => 
        _nombres.ContainsKey(n.Frecuencia) 
            ? _nombres[n.Frecuencia].Count 
            : 0.0;

    protected Dictionary<double, List<Nombre>> _nombres { get; set; }
    public IEnumerable<Nombre> Nombres => _nombres.Values.SelectMany(list => list);

    internal Designacion(List<Nombre> nombres)
    {
        Id = Guid.NewGuid();
        _nombres = nombres
            .GroupBy(n => n.Frecuencia)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    internal Designacion(string texto, Func<string, string> obtenerVerboNucleo)
    {
        Id = Guid.NewGuid();
        _nombres = new Dictionary<double, List<Nombre>>();

        var predicados = texto
            .Split('.')
            .Select(p => p.Trim())
            .Where(p => !string.IsNullOrEmpty(p))
            .ToList();
        var deltaFasePredicados = 2 * Math.PI / predicados.Count;
        var diccionarioVerbos = predicados
            .Select(p => obtenerVerboNucleo(p))
            .GroupBy(p => p)
            .ToDictionary(g => g.Key, g => g.Count());
        
        var palabras = predicados.SelectMany(p => p.Split(' ')).ToList();
        var diccionarioComplementos = palabras
            .Where(p => !diccionarioVerbos.ContainsKey(p)) //Solo los complementos
            .GroupBy(p => p)
            .ToDictionary(g => g.Key, g => Math.Max(1,g.Count()));

        for(var i = 0; i < predicados.Count; i++)
        {
            var palabrasPredicado = predicados[i].Split(' ');
            var verboNucleo = obtenerVerboNucleo(predicados[i]);

            var frecuencia = diccionarioVerbos[verboNucleo];
            if(!_nombres.ContainsKey(frecuencia))
            {
                _nombres.Add(frecuencia, new List<Nombre>());
            }
            var amplitud = palabrasPredicado
                .Where(p => p != verboNucleo)
                .Sum(p => diccionarioComplementos[p]);
            var fase = i * deltaFasePredicados;

            var nombre = new Nombre(predicados[i], 
                fase, 
                frecuencia,
                amplitud,
                this);            
            _nombres[nombre.Frecuencia].Add(nombre);
        }
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

    public static Designacion Vacuidad = new Designacion(new List<Nombre>());
}
