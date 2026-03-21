namespace DomainLogic.Services.Particulas;

public class Electron : Particula
{
    /// <summary>
    /// Crea un electron asociado a un efecto específico
    /// </summary>
    internal Electron(Apariencia efecto) : base(efecto.NaturalezaAparente)
    {
        
    }

    public override double Velocidad => 0.25; // Velocidad del electrón normalizada
    public override double Carga => -1.0;
}
