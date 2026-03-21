using System;
using System.Linq;

public class Nombre
{
    public Guid Id { get; }
    public Palabra Naturaleza { get; }
    public Designacion Causa { get; private set; }
    public Apariencia Efecto { get; private set; }
    public string Texto { get; internal set; }    
    public virtual double Posicion => Efecto
        .Amplitud;
    public virtual double Direccion => Efecto
        .EfectoPrincipal
        .Naturaleza
        .Fase; 
    public virtual double Velocidad
    {
        get
        {
            var sumaDiferencias = Efecto
                .Efectos
                .Zip(Efecto.Efectos.Skip(1), (a, b) => Math.Abs(a.Causa.Frecuencia - b.Causa.Frecuencia))
                .Sum();
            var denominador = Math.Max(sumaDiferencias, double.Epsilon); //Inversamente proporcional a la suma de las diferencias de frecuencia entre las naturalezas
            
            return Efecto.Efectos.Count 
                / denominador;
        }
    }
    
    internal Nombre(string sustantivo, Palabra naturaleza)
    {
        Id = Guid.NewGuid();
        Naturaleza = naturaleza;
        Texto = sustantivo;
    }

    /// <summary>
    /// Permite la herencia al copiar las propiedades de otro nombre.
    /// Para crear el original usar el metodo estático Crear de Designacion.
    /// </summary>
    /// <param name="nombre">El nombre del cual se copiarán las propiedades.</param>
    public Nombre(Nombre nombre)
    {
        Id = nombre.Id;
        Naturaleza = nombre.Naturaleza;
        Causa = nombre.Causa;
        Efecto = nombre.Efecto;
        Texto = nombre.Texto;
    }

    /// <summary>
    /// Muestra una apariencia dependiendo de la designación proyectada.
    /// Se produce algo similar a la modulación AM, FM y PM en secuencia.
    /// La designación proyectada crea una nueva apariencia modulada que depende principalmente del efecto de este nombre, 
    /// a su vez esta modula la frecuencia de la nueva designación 
    /// y la fase del nuevo nombre.
    /// Sobreescribir los métodos Modular de Apariencia, Palabra y Designacion para un comportamiento mas detallado.
    /// </summary>
    /// <param name="designacion">La designación proyectada</param>
    /// <param name="predicado">El predicado que se le asigna a la nueva apariencia</param>
    /// <returns>La apariencia resultante</returns>
    public Apariencia Mostrarse(Designacion designacion, string predicado)
    {
        var nuevaApariencia = new Apariencia(this);
        if(Efecto == null)
        {     
            Causa = designacion;
            Efecto = nuevaApariencia;
            return Efecto;
        }

        //Modulación QAM (similar a OFDM)
        nuevaApariencia.Amplitud = designacion.Naturaleza.Modular(this);
        nuevaApariencia.Efectos = designacion.Naturaleza.Efectos;
        //Modulación FM
        designacion.Modular(this, predicado);
        return nuevaApariencia;
    }

    /// <summary>
    /// Retorna una representación del nombre.
    /// </summary>
    /// <returns>Naturaleza, fase y frecuencia.</returns>
    public override string ToString()
    {
        return $"Nombre: {Texto}, Naturaleza: {Naturaleza.Texto}, Fase: {Naturaleza.Fase * (180 / Math.PI):F2}º, Frecuencia: {Causa.Frecuencia:F2} Hz";
    }
    
    /// <summary>
    /// Sobreescribe GetHashCode para comparar nombres por su Id.
    /// </summary>
    /// <returns>El hash code del nombre.</returns>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

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
}
