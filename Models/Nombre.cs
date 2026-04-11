using System;
using System.Collections.Generic;
using System.Linq;

public class Nombre : Palabra
{
    public override Guid Id { get; }
    public Dictionary<double, List<Apariencia>> Efecto { get; internal set; }
    public double Amplitud => Efecto.Sum(a => a.Value.Sum(e => e.Amplitud));

    internal Nombre(string texto, 
        double fase) 
        : base(texto, fase)
    {
        Id = Guid.NewGuid();
        Efecto = new Dictionary<double, List<Apariencia>>
        {
            { 0, new List<Apariencia> { new Apariencia(this, 0.0) } }
        };
    }

    /// <summary>
    /// Permite la herencia al copiar las propiedades de otro nombre.
    /// Para crear el original usar el metodo estático Aparecer de Apariencia.
    /// </summary>
    /// <param name="nombre">El nombre del cual se copiarán las propiedades.</param>
    public Nombre(Nombre nombre) 
        : base(nombre.Texto, 
            nombre.Fase)
    {
        Id = nombre.Id;
        Efecto = nombre.Efecto;
    }

    /// <summary>
    /// Crea una nueva designación con los nombres seleccionados del espacios según la ventana especificada.
    /// La velocidad de grupo se determina promediando las velocidades de grupo de las apariencias proyectadas.
    /// El espacio designa el nombre tomando la nueva designación como apariencia.
    /// </summary>
    /// <param name="apariencia">La apariencia que funciona como espacio.</param>
    public Designacion Mostrarse(Apariencia apariencia)
    {
        var nuevaFrecuencia = Efecto.Keys.Max() + 1;
        Efecto.Add(nuevaFrecuencia, new List<Apariencia> { apariencia });
        var designacion = apariencia as Designacion;
        if (designacion != null)
        {
            return designacion.Designar(apariencia, this);
        }
        return new Designacion(new List<Nombre> { this });
    }

    /// <summary>
    /// Retorna una representación del nombre.
    /// </summary>
    /// <returns>Naturaleza, fase y frecuencia.</returns>
    public override string ToString() => $"{Texto} ({Fase * (180 / Math.PI):F2}º, {Amplitud:F2} A)";

    /// <summary>
    /// Sobreescribe Equals para comparar nombres por su Id.
    /// </summary>
    /// <returns>True si los nombres son iguales, false en caso contrario.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Nombre other)
        {
            return Id == other.Id;
        }
        return false;
    }

    /// <summary>
    /// Sobreescribe GetHashCode para comparar nombres por su Id.
    /// </summary>
    /// <returns>El hash code del nombre.</returns>
    public override int GetHashCode() => Id.GetHashCode();
}
