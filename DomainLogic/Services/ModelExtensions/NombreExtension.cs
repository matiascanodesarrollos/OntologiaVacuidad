using System.Threading.Tasks;
using DomainLogic.Services.Behaviors;
using MediatR;
using Microsoft.Extensions.Logging;

public static class NombreExtension
{
    /// <summary>
    /// Simula vibración natural del nombre usando el comportamiento especificado
    /// Por defecto utiliza PlasmaBehavior
    /// </summary>
    public static async Task Vibrar(this Nombre nombre, IMediator mediator, ILogger logger, IBehavior behavior = null)
    {
        behavior ??= new PlasmaBehavior();
        await behavior.Execute(nombre, mediator, logger);
    }
}