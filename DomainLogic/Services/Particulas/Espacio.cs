using System.Collections.Generic;
using System.Linq;

namespace DomainLogic.Services.Particulas;

public class Espacio : Nombre
{
    public double Tiempo { get; private set; }
    public Designacion Designacion { get; private set; }
    public List<Particula> Particulas { get; private set; }


    public Espacio(Designacion designacion) 
        : base(designacion.Nombres.First())
    {
        Particulas = designacion.Nombres.Select(n => new Particula(n)).ToList();
        Designacion = designacion;
        Tiempo = 0.0;
    }           

    public void MoverParticulas(double deltaTime)
    {
        Tiempo += deltaTime;
        foreach (var particula in Particulas)
        {
            particula.Mover(deltaTime);
        }

        foreach (var grupo in Particulas.GroupBy(p => p.Posicion2D).Where(g => g.Count() > 1))
        {
            var nuevasDesignaciones = grupo
                .Zip(grupo.Skip(1), (p1, p2) => Designacion.Designar(p1, p2.Causa.Esencia.Apariencia))
                .ToList();
            foreach (var designacion in nuevasDesignaciones)
            {
                var nuevaParticula = new Particula(designacion.Esencia.Nombre);
                Particulas.Add(nuevaParticula);
            }
        }
    }
}
