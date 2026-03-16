using System;
using System.Collections.Generic;

namespace DomainLogic.Services.Particulas;

public class Electron : Particula
{
    public Espacio Espacio { get; private set; }
    public Vector2D UltimaPosicion { get; private set; }
    
    /// <summary>
    /// Crea un electron asociado a un efecto específico
    /// </summary>
    public Electron(Nombre efecto, Espacio espacio) : base(efecto.Causa)
    {
        Espacio = espacio;
        UltimaPosicion = new Vector2D(0, 0);
        var vector = new Vector2D(0, 0);
        if(Espacio.Particulas.ContainsKey(vector))
        {
            Espacio.Particulas[vector].Add(this);
        }
        else
        {
            Espacio.Particulas.Add(
                vector, 
                new List<Particula> { new Foton(Causa), this });
        }    
    }

    public override double Velocidad => 0.9; // Velocidad del electrón normalizada a 0.9c
    public override double Carga => -1.0;
    
    // Usa la Naturaleza del efecto original para calcular la velocidad 2D
    public Vector2D Velocidad2D
    {
        get
        {
            var velocidadBase = Velocidad;
            var fase = Naturaleza.Fase; // Usar la fase del efecto original
            
            var vx = velocidadBase * Math.Cos(fase);
            var vy = velocidadBase * Math.Sin(fase);
            
            return new Vector2D(vx, vy);
        }
    }

    public override void Mover(double deltaTime)
    {
        // Usa velocidad2D con la fase del efecto original
        var velocidadActualizada = Velocidad2D.Suma(Aceleracion.Escala(deltaTime));
        Posicion2D = Posicion2D.Suma(velocidadActualizada.Escala(deltaTime));
        Tiempo += deltaTime;

        // Elimino el electrón de su última posición en el espacio
        Espacio
            .Particulas[UltimaPosicion]
            .Remove(this);

        //Actualizo última posición y agrego el electrón en ella
        UltimaPosicion = new Vector2D((int)Posicion2D.X, (int)Posicion2D.Y);
        if(Espacio.Particulas.ContainsKey(UltimaPosicion))
        {
            Espacio
                .Particulas[UltimaPosicion]
                .Add(this);
            return;
        }

        Espacio.Particulas.Add(
                UltimaPosicion, 
                new List<Particula> { new Foton(Causa), this });
    }
}
