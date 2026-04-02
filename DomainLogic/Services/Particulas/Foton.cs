namespace DomainLogic.Services.Particulas;

public class Foton : Particula
{    
    internal Foton(Nombre nombre) : base(nombre) 
    {
    }
    public override double Masa => 0.0;
    public override double Fase => 0.0;
}
