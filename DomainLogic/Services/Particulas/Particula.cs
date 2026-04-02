using System;
using System.Linq;

namespace DomainLogic.Services.Particulas;

public class Particula : Nombre
{
    public double Tiempo { get; protected set; } 
    public Vector2D Posicion2D { get; protected set; }
    public virtual Vector2D Velocidad2D { get; protected set; }
    public Vector2D Aceleracion { get; protected set; }

    internal Particula(Nombre nombre) : base(nombre)
    {
        var designacion = nombre.Efecto as Designacion;
        Posicion2D = new Vector2D(0, 0);
        var primeraVelocidad = designacion?.Velocidad.FirstOrDefault();
        Velocidad2D = new Vector2D(primeraVelocidad?.x ?? 0, primeraVelocidad?.y ?? 0);
        Aceleracion = new Vector2D(0, 0);
        Tiempo = 0;
    }

    public virtual double Carga => 0.0;
    public virtual double Masa => 1.0;
    public virtual double Energia => Masa * Frecuencia; // E = h * f
    public virtual double Velocidad => 1.0;

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

