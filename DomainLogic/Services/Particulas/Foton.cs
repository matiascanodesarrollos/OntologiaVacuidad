namespace DomainLogic.Services.Particulas;

public class Foton : Particula
{    
    public Foton(Designacion designacion) : base(designacion) 
    {
    }

    public override double Velocidad => 1.0; // Velocidad de la luz normalizada
    public override double Masa => 0.0;
    public double Energia => Causa.Frecuencia; // E = h * f, con h normalizado a 1
}
