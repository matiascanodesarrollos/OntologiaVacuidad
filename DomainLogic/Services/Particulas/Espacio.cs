using System.Collections.Generic;

namespace DomainLogic.Services.Particulas;

public class Espacio : Nombre
{
    public Dictionary<Vector2D, List<Particula>> Particulas { get; private set; }

    public Espacio(Designacion designacion) : base(designacion.Nombre)
    {
        Particulas = new Dictionary<Vector2D, List<Particula>>();
    }
}
