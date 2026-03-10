using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DomainLogic.Services.Plasma;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DomainLogic.Services.ModelExtensions
{
    public static class NombrePlasmaExtensions
    {
    public static Task<PlasmaRunResult> VibrarComoPlasma(
            this Nombre nombre,
            IMediator mediator,
            ILogger logger,
            PlasmaInteractionField field,
            CancellationToken cancellationToken = default)
        {
            var behavior = new PlasmaBehavior();
            return behavior.ExecuteAsync(nombre, mediator, logger, field, cancellationToken);
        }
    }
}
