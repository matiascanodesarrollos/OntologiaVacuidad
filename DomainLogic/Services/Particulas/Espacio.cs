using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainLogic.Services.Particulas;

public class Espacio : Nombre
{
    public double Tiempo { get; private set; }
    public Designacion Designacion { get; private set; }
    public List<Particula> Particulas { get; private set; }
    public List<double> Frecuencias { get; private set; }
    public Dictionary<Particula, List<(double Amplitud, double Fase)>> Ondas { get; private set; }


    public Espacio(Designacion designacion, List<double> frecuencias) 
        : base(designacion.Nombres.First())
    {
        Particulas = designacion.Nombres.Select(n => new Particula(n)).ToList();
        Frecuencias = frecuencias;
        Designacion = designacion;
        Tiempo = 0.0;
        CalcularOndas();
    }           

    public void MoverParticulas(double deltaTime)
    {
        Tiempo += deltaTime;
        foreach (var particula in Particulas)
        {
            particula.Mover(deltaTime);
        }
        foreach (var grupo in Particulas
            .GroupBy(p => p.Posicion2D))
        {
            if(grupo.Count() == 1)
            {
                continue;
            }

            var lista = grupo
                .Select(p => p.Mostrarse(Designacion))
                .ToList();
            Particulas.AddRange(lista.Select(d => new Particula(d.Nombres.Last())));
        }
        CalcularOndas();
    }

    public void CalcularOndas()
    {
        Ondas = Particulas.ToDictionary(
            p => p,
            p => Frecuencias.Select(f => Designacion.Efectos((Tiempo, f))).ToList());
    }
}
