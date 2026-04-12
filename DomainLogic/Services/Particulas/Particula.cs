using System;
using System.Linq;

namespace DomainLogic.Services.Particulas;

public class Particula : Nombre
{
    public double Tiempo { get; protected set; } 
    public Vector2D Posicion2D { get; protected set; }
    public virtual Vector2D Velocidad2D { get; protected set; }
    public virtual Vector2D Aceleracion2D { get; protected set; }

    internal Particula(Nombre nombre) : base(nombre)
    {
        var designacion = new Designacion(nombre
            .Efecto
            .SelectMany(e => e.Value)
            .Select(a => a.Causa)
            .ToList());
        Posicion2D = new Vector2D(0, 0);
        Velocidad2D = new Vector2D(Math.Cos(nombre.Fase), Math.Sin(nombre.Fase));
        Aceleracion2D = new Vector2D(designacion.VelocidadGrupo(0), designacion.VelocidadGrupo(1));
        Tiempo = 0;
    }

    public virtual void Mover(double deltaTime)
    {
        // v = v + a*dt
        Velocidad2D = Velocidad2D.Suma(Aceleracion2D.Escala(deltaTime));
        
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

