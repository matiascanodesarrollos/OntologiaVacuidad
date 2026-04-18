using System;
public class Palabra
{
    public Guid Id { get; }
    public double Fase { get; internal set; }
    public Func<double, double> FuncionFaseInstanea { get; }
    public string Texto { get; }

    internal Palabra(string texto, double fase, Func<double, double> funcionFaseInstanea)
    {
        Id = Guid.NewGuid();
        Texto = texto ?? "Vacuidad";
        Fase = Math.Abs(fase) % (2 * Math.PI);
        FuncionFaseInstanea = funcionFaseInstanea;
    }
}
