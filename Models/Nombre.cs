using System;
using System.Collections.Generic;
using System.Linq;

public class Nombre : Palabra
{
    public override Guid Id { get; }
    public Dictionary<double, List<Apariencia>> Efecto { get; internal set; }
    public double Frecuencia => Efecto.Keys.Max(k => Math.Abs(k));

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
    /// Retorna la amplitud total del nombre para una frecuencia dada, sumando las amplitudes de todas las apariencias asociadas a esa frecuencia.
    /// </summary>
    /// <param name="frecuencia">La frecuencia para la cual se desea obtener la amplitud.</param>
    /// <returns>La amplitud total del nombre para la frecuencia especificada.</returns>
    public (double Amplitud, double Fase) ObtenerValor(double frecuencia)
    {
        if (Efecto.TryGetValue(frecuencia, out var apariencias))
        {
            var amplitud = apariencias.Sum(a => a.Amplitud);
            return (amplitud, Fase);
        }

        return (0, Fase);
    }

    /// <summary>
    /// Suma las amplitudes de cada frecuencia.
    /// </summary>
    /// <returns>La amplitud total del nombre.</returns>
    public double ObtenerAmplitudTotal()
    {
        return Efecto.Values.Sum(apariencias => apariencias.Sum(a => a.Amplitud));
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
    public override string ToString() => $"{Texto} ({Fase * (180 / Math.PI):F2}º, {ObtenerValor(Frecuencia).Amplitud:F2} A)";

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
