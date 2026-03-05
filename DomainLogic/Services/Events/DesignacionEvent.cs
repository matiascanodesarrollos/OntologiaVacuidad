using System;
using MediatR;

public class DesignacionEvent : INotification
{
    public Designacion NuevaDesignacion { get; set; }
    public Apariencia Ambiente { get; set; }
    public DateTime OcurridoEn { get; set; }

    public DesignacionEvent(Designacion nuevaDesignacion, Apariencia ambiente)
    {
        NuevaDesignacion = nuevaDesignacion;
        Ambiente = ambiente;
        OcurridoEn = DateTime.UtcNow;
    }
}