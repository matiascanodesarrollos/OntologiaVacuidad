using System;
using MediatR;

public class DesignacionEvent : INotification
{
    public Designacion NuevaDesignacion { get; set; }
    public DateTime OcurridoEn { get; set; }

    public DesignacionEvent(Designacion nuevaDesignacion)
    {
        NuevaDesignacion = nuevaDesignacion;
        OcurridoEn = DateTime.UtcNow;
    }
}