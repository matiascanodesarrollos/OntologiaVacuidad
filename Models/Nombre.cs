using System;
using System.Collections.Generic;

public class Nombre : Palabra
{
    public override Guid Id { get; }
    public Apariencia Esencia { get; private set; }

    /// <summary>
    /// Función que determina la amplitud dada una frecuencia.
    /// </summary>
    public Func<double, double> Amplitud { get; internal set; }

    internal Nombre(string texto, 
        double fase,
        Func<double, double> amplitud)
        : base(texto, fase)
    {
        Id = Guid.NewGuid();
        Amplitud = amplitud;
        Esencia = new Apariencia(amplitud(0));
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
        Amplitud = nombre.Amplitud;
        Esencia = nombre.Esencia;
    }

    /// <summary>
    /// Crea una nueva designación con los nombres seleccionados del espacios según la ventana especificada.
    /// La velocidad de grupo se determina promediando las velocidades de grupo de las apariencias proyectadas.
    /// El espacio designa el nombre tomando la nueva designación como apariencia.
    /// </summary>
    /// <param name="apariencia">La apariencia que funciona como espacio.</param>
    public Designacion Mostrarse(Apariencia apariencia)
    {
        var designacion = apariencia as Designacion;
        if (designacion != null)
        {
            return designacion.Designar(apariencia, this, t => Fase);
        }

        return new Designacion(new List<Nombre> { this }, x => (0.0, 0.0), f => 1.0);
    }

    /// <summary>
    /// Representacion en texto de la apariencia. 
    /// </summary>
    /// <returns>Una cadena que representa la apariencia.</returns>
    public override string ToString() => $"{Texto} ({Fase * (180 / Math.PI):F2}º, {Esencia.Amplitud:F2} A)";

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
