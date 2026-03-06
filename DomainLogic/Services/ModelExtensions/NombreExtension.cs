using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using DomainLogic.Services.Behaviors;
using MediatR;
using Microsoft.Extensions.Logging;

public static class NombreExtension
{
    // Contexto espacial por nombre (temporal durante vibración)
    private static readonly ConcurrentDictionary<Guid, double> _nombrePositions = 
        new ConcurrentDictionary<Guid, double>();

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