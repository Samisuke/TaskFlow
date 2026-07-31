using TaskFlow.Core.Models;

namespace Taskflow.Core.Repositories
{
    public interface ITareaEtiquetaRepository
    {
        // GET
        Task <IEnumerable<TareaEtiqueta>> ObtenerEtiquetasDeUnaTareaAsync(int idTarea);

        // POST
        Task CrearTareaEtiquetaAsync(TareaEtiqueta tareaEtiqueta);

        // Misc.
        Task <bool> GuardarCambiosAsync();
        Task <bool> ExisteRelacionAsync(int tareaId, int etiquetaId);
    }
}