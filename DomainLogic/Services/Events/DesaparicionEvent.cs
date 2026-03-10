using System;
using System.Collections.Generic;
using MediatR;

namespace DomainLogic.Services.Events
{
    /// <summary>
    /// Evento emitido cuando una vibración desaparece (amplitud se vuelve negligible)
    /// </summary>
    public class DesaparicionEvent : INotification
    {
        public Nombre NombreOrigen { get; set; }
        public DateTime OcurridoEn { get; set; }
        public Stack<Designacion> Stack { get; }

        public DesaparicionEvent(Nombre nombreOrigen, Stack<Designacion> stack)
        {
            NombreOrigen = nombreOrigen;
            OcurridoEn = DateTime.UtcNow;
            Stack = stack;
        }
    }
}
