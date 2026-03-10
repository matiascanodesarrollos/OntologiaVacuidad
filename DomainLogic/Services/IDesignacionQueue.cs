using System.Collections.Generic;

namespace DomainLogic.Services
{
    /// <summary>
    /// Cola para acumular nuevas designaciones generadas durante la vibración.
    /// Permite procesar todas las causas de una designación antes de agregar nuevas.
    /// </summary>
    public interface IDesignacionQueue
    {
        void Enqueue(Designacion designacion);
        List<Designacion> DequeueAll();
        int Count { get; }
    }

    public class DesignacionQueue : IDesignacionQueue
    {
        private readonly List<Designacion> _queue = new List<Designacion>();

        public void Enqueue(Designacion designacion)
        {
            lock (_queue)
            {
                _queue.Add(designacion);
            }
        }

        public List<Designacion> DequeueAll()
        {
            lock (_queue)
            {
                var result = new List<Designacion>(_queue);
                _queue.Clear();
                return result;
            }
        }

        public int Count
        {
            get
            {
                lock (_queue)
                {
                    return _queue.Count;
                }
            }
        }
    }
}
