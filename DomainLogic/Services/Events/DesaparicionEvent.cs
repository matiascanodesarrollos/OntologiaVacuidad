using System;
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

        public DesaparicionEvent(Nombre nombreOrigen)
        {
            NombreOrigen = nombreOrigen;
            OcurridoEn = DateTime.UtcNow;
        }
    }
}
