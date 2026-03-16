using System;
using System.Linq;

public class Nombre
{
    public Guid Id { get; }
    public Palabra Naturaleza { get; }
    public Designacion Causa { get; private set; }
    public Apariencia Efecto { get; private set; }
    public string Texto { get; internal set; }

    public virtual int Posicion => Efecto
        .Amplitud;
    public virtual double Direccion => Efecto
        .Efectos
        .OrderByDescending(n => n.Causa.Apariencia.Amplitud)
        .First()
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
    /// La designación proyectada crea una nueva apariencia modulada(FM) que depende principalmente del efecto de este nombre, 
    /// a su vez esta modula la frecuencia de la nueva designación 
    /// y la fase del nuevo nombre.
    /// </summary>
    /// <param name="designacion">La designación proyectada</param>
    /// <param name="predicado">El predicado que se le asigna a la nueva apariencia</param>
    /// <returns>La apariencia resultante</returns>
    public Apariencia Mostrarse(Designacion designacion, string predicado)
    {
        var nuevaApariencia = new Apariencia(designacion, this);
        if(Efecto == null)
        {     
            Causa = designacion;
            Efecto = nuevaApariencia;
            return Efecto;
        }

        //Modulación AM
        nuevaApariencia.Amplitud = designacion.Apariencia.Modular(this);
        nuevaApariencia.Efectos = designacion.Apariencia.Efectos;
        //Modulación FM
        nuevaApariencia.Naturaleza.Frecuencia = designacion.Modular(this, predicado);
        //Modulación PM
        nuevaApariencia.Naturaleza.Nombre.Naturaleza.Fase = designacion.Nombre.Naturaleza.Modular(Naturaleza.Fase);
        
        return nuevaApariencia;
    }

    public override string ToString()
    {
        return $"Nombre: {Texto}, Naturaleza: {Naturaleza.Texto}, Fase: {Naturaleza.Fase * (180 / Math.PI):F2}º, Frecuencia: {Causa.Frecuencia:F2} Hz";
    }
}
