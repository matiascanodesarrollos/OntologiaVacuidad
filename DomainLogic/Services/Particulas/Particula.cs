using System;

namespace DomainLogic.Services.Particulas;

public class Particula : Nombre
{
    public double Tiempo { get; protected set; } 
    public Vector2D Posicion2D { get; protected set; }
    public Vector2D Aceleracion { get; protected set; }

    internal Particula(Nombre nombre) : base(nombre)
    {
        Posicion2D = new Vector2D(0, 0);
        Aceleracion = new Vector2D(0, 0);
        Tiempo = 0;
    }

    // Velocidad 2D calculada basada en la Fase de la Naturaleza
    public virtual Vector2D Velocidad2D
    {
        get
        {
            var vx = Velocidad * Math.Cos(Naturaleza.Fase);
            var vy = Velocidad * Math.Sin(Naturaleza.Fase);
            
            return new Vector2D(vx, vy);
        }
    }

    public virtual double Carga => 0.0; // Carga por defecto, puede ser sobreescrita por partículas específicas
    public virtual double Masa => 1.0; // Masa por defecto, puede ser sobreescrita por partículas específicas
    public virtual double Energia => Masa * Causa.Frecuencia; // E = h * f

    public virtual void Mover(double deltaTime)
    {
        // Integración usando método de Euler (2D)
        // v = v + a*dt (la velocidad se recalcula automáticamente desde Fase)
        var velocidadActualizada = Velocidad2D.Suma(Aceleracion.Escala(deltaTime));
        
        // p = p + v*dt
        Posicion2D = Posicion2D.Suma(velocidadActualizada.Escala(deltaTime));
        
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

