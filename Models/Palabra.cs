using System;
public class Palabra
{
    public Guid Id { get; }
    public double Fase { get; internal set; }
    public string Texto { get; internal set; }

    internal Palabra(string texto, double fase)
    {
        Id = Guid.NewGuid();
        Texto = texto;
        Fase = Normalizar(fase);
    }
    
    private double Normalizar(double fase)
    {
        return Math.Abs(fase) % (2 * Math.PI);
    }
    
    /// <summary>
    /// Cambia la fase de la palabra si la nueva fase es diferente de la actual.
    /// Se produce algo similar a la modulación PM.
    /// Sobreescribir para un comportamiento mas detallado.
    /// </summary>
    /// <param name="fase">La nueva fase que se utilizará para modular la palabra.</param>
    /// <returns>La nueva fase de la palabra después de la modulación.</returns>
    public virtual double Modular(double fase)
    {
        if(Fase != fase)
        {
            Fase = Normalizar(Fase + fase); // Modulación PM, simula la suma de fases en la función de onda portadora
        }
        
        return Fase;
    }
}
