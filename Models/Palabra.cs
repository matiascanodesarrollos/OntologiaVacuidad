using System;
public class Palabra
{
    public Guid Id { get; }
    public double Fase { get; internal set; }
    public Func<double, double> FaseInstanea { get; }
    public string Texto { get; }

    internal Palabra(string texto, double fase, Func<double, double> faseInstanea)
    {
        Id = Guid.NewGuid();
        Texto = texto ?? Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        Fase = Math.Abs(fase) % (2 * Math.PI);
        FaseInstanea = faseInstanea;
    }

    /// <summary>
    /// Sobreescribe GetHashCode para comparar palabras por su Id.
    /// </summary>
    /// <returns>El hash code de la palabra.</returns>
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    /// Sobreescribe Equals para comparar palabras por su Id.
    /// </summary>
    /// <returns>True si las palabras son iguales, false en caso contrario.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Palabra other)
        {
            return Id == other.Id;
        }
        return false;
    }
}
