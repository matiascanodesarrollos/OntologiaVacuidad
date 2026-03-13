using System;
public class Palabra
{
    public Guid Id { get; }
    public double Fase { get; private set; }
    public string Texto { get; }

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

    private double Normalizar(double fase)
    {
        return fase % (2 * Math.PI);
    }
}
