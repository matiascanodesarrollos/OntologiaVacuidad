namespace DomainLogic.Services.Particulas;

public class Electron : Particula
{
    /// <summary>
    /// Crea un electron asociado a un efecto específico
    /// </summary>
    internal Electron(Nombre efecto) : base(efecto)
    {
        
    }
    
    public override double Carga => -1.0;
}
