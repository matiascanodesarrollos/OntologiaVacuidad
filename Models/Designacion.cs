using System;
using System.Text;
public class Designacion
{
    public Guid Id { get; }    
    public string Verbo { get; }
    public Nombre Nombre { get; private set; }
    public Apariencia Apariencia { get; private set; }    
    public double Frecuencia { get; set; }

    private Designacion(Nombre nombre, Apariencia apariencia, string verbo, double frecuencia)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        Apariencia = apariencia;
        Verbo = verbo;        
        Frecuencia = frecuencia;
    }

    public static Designacion Crear(string adjetivo, string nombre, string verbo, double fase, double frecuencia)
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

    public static Designacion Designar(Nombre nombre, Apariencia apariencia, string sustantivo = null, double fase = 0, double frecuencia = 1)
    {
        nombre.Efecto.Esencia.Frecuencia++;

        var nuevaDesignacion = Crear(nombre.Naturaleza.Texto, 
            sustantivo ?? nombre.Texto,
            nombre.Texto,
            fase,
            frecuencia);
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
