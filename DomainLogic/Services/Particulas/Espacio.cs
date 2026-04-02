using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DomainLogic.Services.Particulas;

public class Espacio : Nombre
{
    public Dictionary<Vector2D, List<Particula>> Particulas { get; private set; }

    internal Espacio(Designacion designacion) : base(designacion.Causa)
    {
        Particulas = new Dictionary<Vector2D, List<Particula>>();
        AgregarParticulasDesignacion(designacion);
    }

    private void AgregarParticulasDesignacion(Designacion designacion)
    {
        AgregarParticula(new Foton(designacion.Causa));
        foreach (var efecto in designacion.Nombres)
        {
            AgregarParticula(new Electron(efecto));
        }
    }

    public static Espacio Crear(Designacion designacion)
    {
        return new Espacio(designacion);
    }

    public void AgregarParticula(Particula particula)
    {
        var posicion = particula.Posicion2D;
        if (Particulas.ContainsKey(posicion))
        {
            Particulas[posicion].Add(particula);
            return;
        }
        
        Particulas[posicion] = new List<Particula> { particula };
    }

    public void MoverParticulas(double deltaTime)
    {
        var posiciones = new List<(Vector2D UltimaPosicion, Vector2D NuevaPosicion, Particula Particula)>();
        foreach (var particulaList in Particulas.Values)
        {
            foreach (var particula in particulaList)
            {
                var ultimaPosicion = particula.Posicion2D;
                particula.Mover(deltaTime);
                posiciones.Add((ultimaPosicion, particula.Posicion2D, particula));
            }
        }

        Particulas = new Dictionary<Vector2D, List<Particula>>();
        foreach (var posicion in posiciones)
        {
            if (Particulas.ContainsKey(posicion.NuevaPosicion))
            {
                var lista = Particulas[posicion.NuevaPosicion];
                var nuevaDesignacion = Apariencia.Aparecer(
                    new List<string>() { posicion.Particula.Texto },
                    x => (posicion.Particula.Fase, 
                        posicion.Particula.Frecuencia, 
                        posicion.Particula.Efecto.Amplitud)) as Designacion;
                lista.ForEach(p => Designacion.Designar(p, nuevaDesignacion));
                AgregarParticulasDesignacion(nuevaDesignacion);
                lista.Add(posicion.Particula);
                continue;
            }
            
            Particulas[posicion.NuevaPosicion] = new List<Particula> { posicion.Particula };
        }
    }
}
