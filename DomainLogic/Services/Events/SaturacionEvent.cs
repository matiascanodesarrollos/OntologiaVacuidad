using System;
using System.Collections.Generic;
using MediatR;

namespace DomainLogic.Services.Events
{
    /// <summary>
    /// Evento emitido cuando una vibración alcanza saturación de amplitud
    /// </summary>
    public class SaturacionEvent : INotification
    {
        public Nombre NombreOrigen { get; set; }
        public DateTime OcurridoEn { get; set; }
        public Stack<Designacion> Stack { get; }

        public SaturacionEvent(Nombre nombreOrigen, Stack<Designacion> stack)
        {
            NombreOrigen = nombreOrigen;
            OcurridoEn = DateTime.UtcNow;
            Stack = stack;
        }
    }
}
