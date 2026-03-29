using System;
public class Palabra
{
    public virtual Guid Id { get; }
    public virtual double Fase { get; private set; }
    public virtual string Texto { get; }
    private static Func<double, double> Normalizar => fase => Math.Abs(fase) % (2 * Math.PI);

    internal Palabra(string texto, double fase)
    {
        Id = Guid.NewGuid();
        Texto = texto ?? Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        Fase = Normalizar(fase);
    }

    /// <summary>
    /// Suma la fase proporcionada a la fase actual de la palabra.
    /// Se produce algo similar a la modulación PM.
    /// Sobreescribir para un comportamiento mas detallado.
    /// </summary>
    /// <param name="palabraProyectada">La palabra que se utilizará para modular la palabra actual.</param>
    public virtual void Modular(Palabra palabraProyectada)
    {
        if(Fase == 0 && palabraProyectada.Fase == 0)
        {
            palabraProyectada.Fase = Math.PI / 2; //Si ambas palabras aparentar ser las iniciales la original empuja la otra al plano imaginario.
            return;
        }
        Fase = Normalizar(Fase + palabraProyectada.Fase); // Modulación PM, simula la suma de fases en la función de onda portadora        
    }
}
