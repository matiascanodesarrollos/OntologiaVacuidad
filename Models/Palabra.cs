using System;
public class Palabra
{
    public Guid Id { get; }
    public double Fase { get; internal set; }
    public string Texto { get; }

    internal Palabra(string texto, double fase)
    {
        Id = Guid.NewGuid();
        Texto = texto ?? "Vacuidad";
        Fase = Math.Abs(fase) % (2 * Math.PI);
    }
}
