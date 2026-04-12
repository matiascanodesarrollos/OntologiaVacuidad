using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainLogic.Services.Particulas;

public class Espacio : Nombre
{
    public List<Particula> Particulas { get; private set; }

    internal Espacio(Designacion designacion) : base(designacion.Causa)
    {
        Particulas = new List<Particula>();
        AgregarParticulasDesignacion(designacion);
    }

    private void AgregarParticulasDesignacion(Designacion designacion)
    {
        foreach (var efecto in designacion.Nombres)
        {
            Particulas.Add(new Particula(efecto));
        }
    }

    public static Espacio Crear(Designacion designacion)
    {
        return new Espacio(designacion);
    }

    public void MoverParticulas(double deltaTime)
    {
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
                    .OrderByDescending(p => p.ObtenerValor(p.Frecuencia).Amplitud)
                    .ToList();
            var apariencia = Apariencia.Aparecer(new List<string>(), null);
            lista.ForEach(p => p.Mostrarse(apariencia));
            AgregarParticulasDesignacion(apariencia as Designacion);
        }
    }
}
