namespace DomainLogic.Services.Particulas;

public class Particula : Nombre
{
    public double Tiempo { get; protected set; } 
    public Vector2D Posicion2D { get; protected set; }
    public virtual Vector2D Velocidad2D { get; protected set; }

    internal Particula(Nombre nombre) : base(nombre)
    {
        var designacion = nombre.Efecto as Designacion;
        Posicion2D = new Vector2D(0, 0);
        Velocidad2D = new Vector2D(designacion.Velocidad.X, designacion.Velocidad.Y);
        Tiempo = 0;
    }

    public virtual void Mover(double deltaTime)
    {        
        // p = p + v*dt
        Posicion2D = Posicion2D.Suma(Velocidad2D.Escala(deltaTime));
        
        Tiempo += deltaTime;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

