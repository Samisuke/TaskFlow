using TaskFlow.Core.Models;

namespace Taskflow.Core.Repositories
{
    public interface IHistorialRepository
    {
        // GET
        Task <IEnumerable<Historial>> ObtenerHistorialDeUnaTareaAsync(int idTarea);

        // POST
        Task CrearHistorialAsync(Historial historial);

        // Misc.
        Task <bool> GuardarCambiosAsync();
    }
}