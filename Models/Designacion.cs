using System;
using System.Text;

public class Designacion
{
    public Guid Id { get; }    
    public string Verbo { get; }
    public Nombre Nombre { get; private set; }
    public Apariencia Apariencia { get; private set; }    
    public double Frecuencia { get; internal set; }

    private Designacion(Nombre nombre, Apariencia apariencia, string verbo, double frecuencia)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        Apariencia = apariencia;
        Verbo = verbo;        
        Frecuencia = frecuencia;
    }

    public static Designacion Imaginar(
        string adjetivo, 
        string nombre, 
        string verbo, 
        double fase, 
        double frecuencia)
    {
        var designacion = new Designacion(null, null, verbo, frecuencia);
        var nuevoNombre = new Nombre(nombre, new Palabra(adjetivo, fase), null);
        var nuevaApariencia = new Apariencia(designacion, nuevoNombre);

        designacion.Nombre = nuevoNombre;
        designacion.Apariencia = nuevaApariencia;

        nuevoNombre.Causa = designacion;
        nuevoNombre.Efecto = nuevaApariencia;

        return designacion;
    }

    public static Designacion Designar(
        Nombre significado, 
        Apariencia apariencia, 
        string sustantivo, 
        double? frecuencia = null, //Designar buscando crear un nuevo significado, por ejemplo cuando un arquitecto diseña una casa.
        double? fase = null //Designar conociendo la vacuidad, se controla por completo como interactua la nueva designación con la base y por lo tanto su apariencia.
    )
    {
        var deltaFrecuencia = !frecuencia.HasValue 
            ? significado.Causa.Frecuencia + 1 //Se asume una frecuencia cercana para que las ondas interactuen.
            : significado.Causa.Frecuencia + frecuencia.Value; //Se controlla durante el diseño como interactua con la onda original.

        var deltaFase = !fase.HasValue 
            ? significado.Naturaleza.Fase + Math.PI / 2 //Se asume desfase de 90º para evitar interferencia y permitir que interactuen.
            : significado.Naturaleza.Fase + fase.Value; //Admite cualquier tipo de interacción.
        deltaFase %= 2 * Math.PI;
        
        var nuevaDesignacion = Imaginar(significado.Naturaleza.Texto, 
            sustantivo,
            $"Parecer {sustantivo}/{significado.Causa.Verbo}",
            deltaFase,
            deltaFrecuencia);
        nuevaDesignacion.Nombre.Causa = apariencia.Esencia;
        nuevaDesignacion.Nombre.Efecto = nuevaDesignacion.Apariencia;

        return nuevaDesignacion;
    }

    public override string ToString()
    {
        var resultado = new StringBuilder();
        resultado.AppendLine("═══ Designación ═══");
        var apariencias = Nombre.BuscarSignificado();
        foreach (var apariencia in apariencias)
        {
            resultado.AppendLine(apariencia.ToString());
        }
        return resultado.ToString();
    }

}
