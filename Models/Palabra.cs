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
    
    internal double Modular(double fase)
    {
        if(Fase != fase)
        {
            Fase = Normalizar(Fase + fase); // Modulación PM, simula la suma de fases en la función de onda portadora
        }
        
        return Fase;
    }

    public static double Normalizar(double fase)
    {
        return Math.Abs(fase) % (2 * Math.PI);
    }
}
