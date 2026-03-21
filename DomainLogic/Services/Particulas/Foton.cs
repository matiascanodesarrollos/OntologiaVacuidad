namespace DomainLogic.Services.Particulas;

public class Foton : Particula
{    
    internal Foton(Designacion designacion) : base(designacion.Esencia) 
    {
    }

    public override double Velocidad => 0.3; // Velocidad de la luz normalizada
    public override double Masa => 0.0;
}
