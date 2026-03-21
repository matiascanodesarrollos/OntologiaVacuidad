using System;
using System.Collections.Generic;
using System.Linq;
public class Apariencia
{
    public Guid Id { get; }
    public double Amplitud { get; internal set; } = 1;
    public IEnumerable<string> EnteDesignado 
    {  
        get
        {
            var efectoPrincipal = EfectoPrincipal;
            return new List<string>
            {
                $"Naturaleza: {NaturalezaAparente.Naturaleza.Texto}",
                $"Causa: {efectoPrincipal.Texto}",            
                $"Frecuencia: {efectoPrincipal.Causa.Frecuencia:F2}",
                $"Fase: {efectoPrincipal.Naturaleza.Fase:F2}°",
                $"Amplitud: {Amplitud}",
                $"Efecto: {efectoPrincipal.Naturaleza.Texto}",
            };
        }
    }
    public Nombre NaturalezaAparente => Efectos.First();
    public Nombre EfectoPrincipal => Efectos.OrderByDescending(e => e.Efecto.Amplitud).First();
    public IList<Nombre> Efectos { get; set; }
    
    internal Apariencia(Nombre causa)
    {
        Id = Guid.NewGuid();
        Efectos = new List<Nombre> { causa };
    }

    /// <summary>
    /// Añade un efecto a la apariencia si designación proyectada esta dentro del rango de frecuencia.
    /// Se produce algo similar a la modulación QAM simplificada (OFDM).
    /// Sobreescribir para un comportamiento mas detallado.
    /// </summary>
    /// <param name="nombreProyectado">El nombre proyectado que se utilizará para modular la apariencia.</param>
    /// <returns>La nueva amplitud de la apariencia después de la modulación.</returns>
    public virtual double Modular(Nombre nombreProyectado)
    {
        var frecuenciaMaxima = Efectos.Max(x => Math.Abs(x.Causa.Frecuencia));
        if(Math.Abs(nombreProyectado.Causa.Frecuencia) <= frecuenciaMaxima)
        {
            if(!Efectos.Any(c => c.Id == nombreProyectado.Id))
            {
                Efectos.Add(nombreProyectado);
                //Modulación PM
                EfectoPrincipal.Naturaleza.Modular(nombreProyectado.Naturaleza.Fase);
            }
            
            //Modulación AM, simula la multiplicación de la función de onda portadora por el mensaje
            Amplitud *= 1 + nombreProyectado.Efecto.Amplitud;
        } 
        return Amplitud;
    }

    /// <summary>
    /// Sobreescribe ToString para mostrar una representación de la apariencia.
    /// </summary>
    /// <returns>Una cadena que representa la apariencia.</returns>
    public override string ToString()
    {
        return $"Apariencia: {string.Join(", ", EnteDesignado)}";
    }
}
