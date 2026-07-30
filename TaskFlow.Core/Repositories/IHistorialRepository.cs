using TaskFlow.Core.Models;

namespace Taskflow.Core.Repositories
{
    public interface IHistorialRepository
    {
        // Peticiones GET
        Task <IEnumerable<Historial>> ObtenerHistorialDeUnaTareaAsync(int idTarea);

        // Misc.
        Task CrearHistorialAsync(Historial historial);
        Task <bool> GuardarCambiosAsync();
    }
}