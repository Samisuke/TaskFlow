using TaskFlow.Core.Common;
using TaskFlow.Core.Models;

namespace TaskFlow.Core.Services
{
    public interface IHIstorialService
    {
        // Métodos GET
        Task<Result<IEnumerable<Historial>>> GetHistorialDeUnaTareaAsync(int idTarea);

        //Métodos POST
        Task<Result<Historial>> PostTareaAsync(
            int tareaId,
            int usuarioId,
            string accion
        );
    }
}