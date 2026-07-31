using TaskFlow.Core.Common;
using TaskFlow.Core.Models;

namespace TaskFlow.Core.Services
{
    public interface ITareaEtiquetaService
    {
        // Métodos GET
        Task<Result<IEnumerable<TareaEtiqueta>>> GetEtiquetasDeUnaTareaAsync(int idTarea);

        // Método POST
        Task<Result<TareaEtiqueta>> PostTareaEtiquetaAsync(
            int idPropia,
            int tareaId,
            string nombreEtiqueta,
            string colorEtiqueta
        );
    }
}