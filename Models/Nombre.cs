using System;
using System.Collections.Generic;
using System.Linq;

public class Nombre : Palabra
{
    public override Guid Id { get; }
    public double Frecuencia { get; internal set; }
    public Apariencia Efecto { get; internal set; }

    internal Nombre(string texto, 
        double fase, 
        double frecuencia) 
        : base(texto, fase)
    {
        Id = Guid.NewGuid();
        Efecto = new Designacion(new List<Nombre>(){ this, });
        Frecuencia = frecuencia;
    }

    /// <summary>
    /// Permite la herencia al copiar las propiedades de otro nombre.
    /// Para crear el original usar el metodo estático Designar de Designacion.
    /// </summary>
    /// <param name="nombre">El nombre del cual se copiarán las propiedades.</param>
    public Nombre(Nombre nombre) 
        : base(nombre.Texto, 
            nombre.Fase)
    {
        Id = nombre.Id;
        Efecto = nombre.Efecto;
        Frecuencia = nombre.Frecuencia;
    }

    /// <summary>
    /// Modula la apariencia agregandose a si mismo a la lista de efectos.
    /// Ademas crea una nueva designación y proyectando los nombres recientes según la ventana especificada.    
    /// </summary>
    /// <param name="apariencia">La designación que funciona como espacio.</param>
    /// <param name="ventana">El número de nombres recientes a considerar en la proyección.</param>
    /// <returns>Nueva designación.</returns>
    public Designacion Mostrarse(Designacion apariencia, int ventana)
    {
        apariencia.Nombres.Add(this);

        var proyeccion = apariencia.Nombres.TakeLast(ventana).ToList();
        var designacion = new Designacion(proyeccion);
        
        return designacion;
    }

    /// <summary>
    /// Retorna una representación del nombre.
    /// </summary>
    /// <returns>Naturaleza, fase y frecuencia.</returns>
    public override string ToString() => $"{Texto} ({Fase * (180 / Math.PI):F2}º, {Frecuencia:F2} Hz, {Efecto.Amplitud:F2} A)";

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
