using System;
public class Palabra
{
    public Guid Id { get; }
    public double Fase { get; }
    public string Texto { get; }

    internal Palabra(string texto, double fase)
    {
        Id = Guid.NewGuid();
        Texto = texto;
        Fase = Normalizar(fase);
    }

    internal double Modular(double fase)
    {
        if(Math.Abs(Fase - fase) <= 0.1)
        {
            return Normalizar(Fase + fase); // Modulación PM
        }

        return Fase;
    }

    private double Normalizar(double fase)
    {
        return fase % (2 * Math.PI);
    }
}
