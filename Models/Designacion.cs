using System;
using System.Collections.Generic;
using System.Linq;

public class Designacion : Apariencia
{
    /// <summary>
    /// Función que determina la velocidad de grupo dada un nombre.
    /// </summary>
    public Func<Nombre, double> VelocidadGrupo { get; }

    internal Designacion(List<Nombre> nombres, 
        Func<Nombre, double> velocidadGrupo)
        : base(Vacuidad.Valor, nombres)
    {
        VelocidadGrupo = velocidadGrupo;
    }

    internal Designacion(string texto, Func<string, string> obtenerVerboNucleo)
        : base(Vacuidad.Valor, Vacuidad.Nombres.ToList())
    {
        VelocidadGrupo = n => 1;

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
            var amplitud = palabrasPredicado
                .Where(p => p != verboNucleo)
                .Sum(p => diccionarioComplementos[p]);
            var fase = i * deltaFasePredicados;

            var nombre = new Nombre(predicados[i], 
                fase, 
                frecuencia);
            var apariencia = new Apariencia(t => 
                (amplitud * Math.Cos(frecuencia * t + fase), 
                frecuencia * Math.Sin(frecuencia * t + fase))
                , new List<Nombre> { nombre });
            nombre.Esencia = apariencia;
            
            _nombres.Add(nombre);
        }
    }

    /// <summary>
    /// Crea una nueva designación al proyectar el nombre sobre la apariencia.
    /// Si la apariencia no es una designación, se toma la designación actual como apariencia.
    /// </summary>
    /// <param name="nombre">El nombre a proyectar.</param>
    /// <param name="palabra">La palabra sobre la cual se proyecta el nombre.</param>
    /// <returns>La nueva designación creada.</returns>
    public Designacion Designar(Nombre nombre, Palabra palabra)
    {        
        var nombres = Nombres.ToList();
        var nombreProyectado = new Nombre(palabra.Texto, palabra.Fase, nombre.Frecuencia);
        nombres.Add(nombreProyectado);
        var nuevaDesignacion = new Designacion(nombres, n =>
        {
            var nombreActual = nombres.Contains(n);
            var velocidad = VelocidadGrupo(n);
            return nombreActual
                ? velocidad 
                : velocidad + 1; //Si el nombre no es el correcto va a una velocidad más alta
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
