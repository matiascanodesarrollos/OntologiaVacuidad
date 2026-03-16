using System;
using System.Linq;
using System.Text;

public class Designacion
{
    public Guid Id { get; }    
    public string Texto { get; internal set; }
    public Nombre Nombre { get; private set; }
    public Apariencia Apariencia { get; private set; }    
    public double Frecuencia { get; internal set; }
    public double AnchoBanda { get; internal set; } = 5;

    private Designacion(string verbo, double frecuencia)
    {
        Id = Guid.NewGuid();
        Texto = verbo;        
        Frecuencia = frecuencia;
    }

    private void Actualizar(string sujeto, string predicado, double frecuencia)
    {
        Frecuencia = frecuencia;        
        var delimitador = ' ';
        var predicadoArray = predicado.Split(delimitador);        
        Texto = predicadoArray.First();
        Nombre.Naturaleza.Texto = string.Join(delimitador, predicadoArray.Skip(1));
        Nombre.Texto = sujeto;        
    }
    
    internal double Modular(
        Nombre significado, 
        string predicado
    )
    {        
        //Modulación FM, simula la integral del mensaje instantáneo
        var frecuenciaModulada = Frecuencia;        
        foreach (var naturaleza in Apariencia.Efectos)
        {
            if(Math.Abs(significado.Causa.Frecuencia - naturaleza.Causa.Frecuencia) <= AnchoBanda)
            {
                frecuenciaModulada = naturaleza.Causa.Frecuencia;
                naturaleza.Causa.Actualizar(significado.Texto, predicado, frecuenciaModulada);
            }
        }
        Actualizar(significado.Texto, predicado, frecuenciaModulada);
        return Frecuencia;
    }


    /// <summary>
    /// Crea una esencia o nueva designación imaginando un nuevo significado para un adjetivo, un sustantivo y un verbo, junto con su fase y frecuencia.
    /// De esta manera se crea una onda portadora que puede ser modulada posteriormente para crear nuevas designaciones a partir de esta.
    /// Se asemeja a la modulación PM [s(φ)=p(φ+m(φ))] en el sentido de crear un "foton" de frecuencia y fase, que luego puede ser modulado por otras designaciones para generar nuevos significados.
    /// </summary>
    /// <param name="sustantivo">Permite derivar la frecuencia</param>
    /// <param name="verbo">Permite derivar la amplitud</param>
    /// <param name="adjetivo">Permite derivar la fase</param>
    /// <param name="frecuencia">Permite controlar la interacción con otras ondas y por lo tanto el significado</param>
    /// <param name="fase">Permite elegir la función de la onda portadora (ej: cos(φ)=sen(φ+90º))</param>
    /// <returns>La nueva designación creada (con su Nombre/Palabra y su Apariencia).</returns>
    public static Designacion Crear(
        string sustantivo, 
        string verbo, 
        string adjetivo,
        double frecuencia,
        double fase)
    {
        var designacion = new Designacion(verbo, frecuencia);
        var nuevaPalabra = new Palabra(adjetivo, fase);
        var nuevoNombre = new Nombre(sustantivo, nuevaPalabra);
        designacion.Apariencia = nuevoNombre.Mostrarse(designacion, $"{verbo} {adjetivo}");
        designacion.Nombre = nuevoNombre;
        return designacion;
    }

    public override string ToString()
    {
        var resultado = new StringBuilder();
        resultado.AppendLine("═══ Designación ═══");
        foreach (var causa in Apariencia.Efectos)
        {
            if(causa.Causa.Frecuencia == 0)
            {
                break;
            }
            resultado.AppendLine(causa.ToString());
        }
        resultado.AppendLine("═══ Fin ═══");
        return resultado.ToString();
    }

}
