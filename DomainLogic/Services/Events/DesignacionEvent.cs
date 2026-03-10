using System;
using System.Collections.Generic;
using MediatR;

public class DesignacionEvent : INotification
{
    public Designacion NuevaDesignacion { get; }
    public DateTime OcurridoEn { get; }
    public Stack<Designacion> Stack { get; }

    public DesignacionEvent(Designacion nuevaDesignacion, Stack<Designacion> stack)
    {
        NuevaDesignacion = nuevaDesignacion;
        OcurridoEn = DateTime.UtcNow;
        Stack = stack;
    }
}