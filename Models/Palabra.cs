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
        Fase = fase;
    }
}
