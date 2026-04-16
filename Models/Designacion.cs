using System;
using System.Collections.Generic;
using System.Linq;

public class Designacion : Apariencia
{
    public override Guid Id { get; }
    private List<Nombre> _nombres { get; set; }
    public IEnumerable<Nombre> Nombres => _nombres.AsReadOnly();

    /// <summary>
    /// Función que determina la velocidad de grupo dada un nombre.
    /// </summary>
    public Func<Nombre, double> VelocidadGrupo { get; }

    internal Designacion(List<Nombre> nombres, 
        Func<Nombre, double> velocidadGrupo)
        : base(Palabra.Vacuidad)
    {
        Id = Guid.NewGuid();
        _nombres = nombres;
        VelocidadGrupo = velocidadGrupo;
    }

    internal Designacion(List<string> predicados)
        : base(Palabra.Vacuidad)
    {
        Id = Guid.NewGuid();
        _nombres = new List<Nombre>();

        var deltaFasePredicados = 2 * Math.PI / predicados.Count;
        var diccionarioVerbos = predicados
            .Select(p => p.Split(' ').First())
            .GroupBy(p => p)
            .ToDictionary(g => g.Key, g => g.Count());
        
        var diccionarioComplementos = predicados
            .SelectMany(p => p.Split(' ').Skip(1))
            .GroupBy(p => p)
            .ToDictionary(g => g.Key, g => Math.Max(1,g.Count()));

        for(var i = 0; i < predicados.Count; i++)
        {
            var palabras = predicados[i].Split(' ');

            var frecuencia = diccionarioVerbos[palabras.First()];
            var amplitud = palabras.Skip(1).Sum(p => diccionarioComplementos[p]);
            var fase = i * deltaFasePredicados;

            var apariencia = new Apariencia(t => 
                amplitud * Math.Cos(frecuencia * Math.PI * t + fase) 
                + amplitud * Math.Sin(frecuencia * Math.PI * t + fase));            
            var nombre = new Nombre(predicados[i], fase, frecuencia, apariencia);
            _nombres.Add(nombre);
        }

        VelocidadGrupo = n => 1;
    }

    /// <summary>
    /// Crea una nueva designación al proyectar el nombre sobre la apariencia.
    /// Si la apariencia no es una designación, se toma la designación actual como apariencia.
    /// </summary>
    /// <param name="apariencia">La apariencia sobre la cual proyectar el nombre.</param>
    /// <param name="nombre">El nombre a proyectar.</param>
    /// <returns>La nueva designación creada.</returns>
    public Designacion Designar(Apariencia apariencia, 
        Nombre nombre)
    {
        var designacion = apariencia as Designacion;
        if (designacion == null)
        {
            designacion = this;
        }
        var nombres = designacion
            .Nombres
            .SkipLast(1) //Analogo a derivar
            .ToList();
        nombres.Add(nombre);
        var nuevaDesignacion = new Designacion(nombres, n =>
        {
            var velocidad = VelocidadGrupo(n);
            return velocidad != 0 
                ? velocidad 
                : velocidad + 2; //Si el nombre no es el correcto va a una velocidad más alta
        });        
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
