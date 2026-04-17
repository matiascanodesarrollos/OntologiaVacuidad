using System.Collections.Generic;
using System.Linq;

namespace DomainLogic.Services.Particulas;

public class Espacio : Nombre
{
    public double Tiempo { get; private set; }
    public Designacion Designacion { get; private set; }
    public List<Particula> Particulas { get; private set; }
    public Dictionary<Particula, List<(double Amplitud, double Fase)>> Ondas { get; private set; }


    public Espacio(Designacion designacion) 
        : base(designacion.Nombres.First())
    {
        Particulas = designacion.Nombres.Select(n => new Particula(n)).ToList();
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
        CalcularOndas();
    }

    public void CalcularOndas()
    {
        Ondas = Particulas.ToDictionary(
            p => p,
            p => p.Mostrarse(Designacion, null)
                .Nombres
                .Select(n => n.Esencia.Amplitud(Tiempo))
                .ToList());
    }
}
