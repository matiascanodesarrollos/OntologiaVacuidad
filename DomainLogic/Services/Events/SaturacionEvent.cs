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

        public SaturacionEvent(Nombre nombreOrigen)
        {
            NombreOrigen = nombreOrigen;
            OcurridoEn = DateTime.UtcNow;
        }
    }
}
