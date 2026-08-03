using TaskFlow.Core.Common;
using TaskFlow.Core.Models;
using TaskFlow.Core.Requests;

namespace TaskFlow.Core.Services
{
    public interface ITareaEtiquetaService
    {
        // Métodos GET
        Task<Result<IEnumerable<TareaEtiqueta>>> GetEtiquetasDeUnaTareaAsync(int idTarea);

        // Método POST
        Task<Result> PostTareaEtiquetaAsync(
            Tarea tarea,
            IEnumerable<NuevaEtiqueta> etiquetas
        );
    }
}