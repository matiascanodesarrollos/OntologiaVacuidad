using System;
public class Palabra
{
    public virtual Guid Id { get; }
    public virtual double Fase { get; internal set; }
    public virtual string Texto { get; }
    public static Func<double, (double, double)> Vacuidad = t => 
        t == 0 
            ? (double.MaxValue, double.MaxValue) 
            : (Math.Cos(t), Math.Sin(t));

    internal Palabra(string texto, double fase)
    {
        Id = Guid.NewGuid();
        Texto = texto;
        Fase = Math.Abs(fase) % (2 * Math.PI);
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
