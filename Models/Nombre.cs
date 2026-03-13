using System;
using System.Linq;

public class Nombre
{
    public Guid Id { get; }
    public Palabra Naturaleza { get; }
    public Designacion Causa { get; private set; }
    public Apariencia Efecto { get; private set; }
    public string Texto { get; }

    public double Posicion => Efecto
        .Amplitud;
    public double Direccion => Efecto
        .Naturalezas
        .OrderByDescending(n => n.Causa.Frecuencia)
        .First()
        .Efecto
        .Amplitud; //Hacia la naturaleza con mayor frecuencia
    public double Velocidad => 1.0 
        / Efecto
            .Naturalezas
            .Zip(Efecto.Naturalezas.Skip(1), (a, b) => Math.Abs(a.Causa.Frecuencia - b.Causa.Frecuencia))
            .Sum(); //Inversamente proporcional a la suma de las diferencias de frecuencia entre las naturalezas
    
    internal Nombre(string sustantivo, Palabra naturaleza)
    {
        Id = Guid.NewGuid();
        Naturaleza = naturaleza;
        Texto = sustantivo;
    }

    /// <summary>
    /// Muestra una apariencia dependiendo de la designación proyectada.
    /// Se produce algo similar a la modulación AM, FM y PM en secuencia.
    /// La designación proyectada crea una nueva apariencia modulada(FM) que depende principalmente del efecto de este nombre, 
    /// a su vez esta modula la frecuencia de la nueva designación 
    /// y la fase del nuevo nombre.
    /// </summary>
    /// <param name="designacion">La designación proyectada</param>
    /// <param name="frecuenciaForzada">La frecuencia frozada opcional</param>
    /// <param name="faseForzada">La fase forzada opcional</param>
    /// <returns>La apariencia resultante</returns>
    public Apariencia Mostrarse(Designacion designacion, double? frecuenciaForzada = null, double? faseForzada = null)
    {        
        if(Efecto == null)
        {
            var nuevaApariencia = new Apariencia(designacion, this);       
            Causa = designacion;
            Efecto = nuevaApariencia;
            return nuevaApariencia;
        }
        
        //Modulación FM
        var nombreProyectado = designacion.Nombre;
        var sujeto = nombreProyectado.Texto;
        var predicado = $"{nombreProyectado.Causa.Texto} {nombreProyectado.Naturaleza.Texto}";
        designacion.Modular(nombreProyectado, Efecto, sujeto, predicado);
        //Modulación AM
        Efecto.Modular(nombreProyectado, frecuenciaForzada);
        //Modulación PM
        Naturaleza.Modular(faseForzada ?? Naturaleza.Fase);
        return Efecto;
    }

    public override string ToString()
    {
        return $"Nombre: {Texto}, Naturaleza: {Naturaleza.Texto}, Fase: {Naturaleza.Fase * (180 / Math.PI):F2}º, Frecuencia: {Causa.Frecuencia:F2} Hz";
    }
}
