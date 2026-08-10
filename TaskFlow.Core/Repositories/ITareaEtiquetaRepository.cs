using TaskFlow.Core.Models;

// Define las operaciones de acceso y persistencia de las relaciones entre tareas y etiquetas.

namespace TaskFlow.Core.Repositories
{
    public interface ITareaEtiquetaRepository
    {
        // GET
        Task <IEnumerable<TareaEtiqueta>> ObtenerEtiquetasDeUnaTareaAsync(int tareaId);

        // POST
        Task CrearTareaEtiquetaAsync(TareaEtiqueta tareaEtiqueta);

        // Misc.
        Task <bool> ExisteRelacionAsync(int tareaId, int etiquetaId);
    }
}