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
        Fase = Normalizar(Fase + fase);
        return Fase; // Modulación PM
    }

    private double Normalizar(double fase)
    {
        return fase % (2 * Math.PI);
    }
}
