using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DomainLogic.Services.Behaviors
{
    /// <summary>
    /// Define el contrato para diferentes comportamientos de Nombre
    /// Permite agregar comportamientos tipo plasma, fluidos, etc.
    /// </summary>
    public interface IBehavior
    {
        /// <summary>
        /// Ejecuta el comportamiento específico del nombre
        /// </summary>
        Task Execute(Nombre nombre, IMediator mediator, ILogger logger);
    }
}
