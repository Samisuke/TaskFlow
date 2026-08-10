using TaskFlow.Core.Models;

// Define las operaciones de acceso y persistencia de datos relacionadas con el historial de los proyectos.

namespace TaskFlow.Core.Repositories
{
    public interface IHistorialRepository
    {
        // GET
        Task <IEnumerable<Historial>> ObtenerHistorialAsync(int proyectoId);

        // POST
        Task CrearHistorialAsync(Historial historial);
    }
}