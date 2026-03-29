using System.Collections.Generic;
using System.Linq;

namespace DomainLogic.Services.Particulas;

public class Espacio : Nombre
{
    public Dictionary<Vector2D, List<Particula>> Particulas { get; private set; }

    internal Espacio(Designacion designacion) : base(designacion.Causa)
    {
        Particulas = new Dictionary<Vector2D, List<Particula>>();
        AgregarParticula(new Foton(designacion.Nombres.First()));
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
        var nuevasApariencias = new List<Apariencia>();
        const int maxNuevasParticulas = 20; // Limitar crecimiento exponencial
        
        foreach (var posicion in posiciones)
        {
            if (Particulas.ContainsKey(posicion.NuevaPosicion))
            {
                var lista = Particulas[posicion.NuevaPosicion];
                
                var interaccionCompleja = lista
                        .Where(p => p.Carga != 0)
                        .ToList();
                        
                // Solo crear apariencias si no hay demasiadas partículas ya
                if(interaccionCompleja.Count > 1 && nuevasApariencias.Count < maxNuevasParticulas)
                {
                    var nuevas = interaccionCompleja.Zip(interaccionCompleja.Skip(1), (a, b) => a.Mostrarse(b.Esencia))
                        .ToList();
                    // Limitar la cantidad añadida en este frame
                    var cantidadAAnadir = System.Math.Min(nuevas.Count, maxNuevasParticulas - nuevasApariencias.Count);
                    nuevasApariencias.AddRange(nuevas.Take(cantidadAAnadir));
                }

                lista.Add(posicion.Particula);                
                continue;
            }
            
            Particulas[posicion.NuevaPosicion] = new List<Particula> { posicion.Particula };
        }

        foreach (var apariencia in nuevasApariencias)
        {
            AgregarParticula(new Electron(apariencia.Causa));
        }
    }
}
