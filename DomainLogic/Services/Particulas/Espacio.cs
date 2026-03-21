using System.Collections.Generic;
using System.Linq;

namespace DomainLogic.Services.Particulas;

public class Espacio : Nombre
{
    public Dictionary<Vector2D, List<Particula>> Particulas { get; private set; }

    internal Espacio(Designacion designacion) : base(designacion.Esencia)
    {
        Particulas = new Dictionary<Vector2D, List<Particula>>();
        foreach (var efecto in designacion.Naturaleza.Efectos)
        {
            AgregarParticula(new Foton(efecto.Causa));
            AgregarParticula(new Electron(efecto.Efecto));
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

            if(!Particulas[posicion].Any(x => x as Foton != null))
            {
                Particulas[posicion].Add(new Foton(Causa));
            }
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
        foreach (var posicion in posiciones)
        {
            if (Particulas.ContainsKey(posicion.NuevaPosicion))
            {
                var lista = Particulas[posicion.NuevaPosicion];
                
                var interaccionCompleja = lista
                        .Where(p => p.Carga != 0)
                        .ToList();
                if(interaccionCompleja.Count > 1)
                {
                    nuevasApariencias.AddRange(
                        interaccionCompleja.Zip(interaccionCompleja.Skip(1), (a, b) => a.Mostrarse(b.Causa, $"{a.Causa.Texto} {b.Naturaleza.Texto}"))
                        .ToList());
                }

                lista.Add(posicion.Particula);
                if(!lista.Any(x => x as Foton != null))
                {
                    lista.Add(new Foton(Causa)); //Simula el hecho de que desde el punto de vista del foton, el espacio se encuentra en todas partes y por lo tanto siempre hay un foton presente para mediar las interacciones.
                }
                
                continue;
            }
            
            Particulas[posicion.NuevaPosicion] = new List<Particula> { posicion.Particula };
        }

        foreach (var apariencia in nuevasApariencias)
        {
            AgregarParticula(new Electron(apariencia));
        }
    }
}
