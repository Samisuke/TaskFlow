using TaskFlow.Core.Common;
using TaskFlow.Core.Models;
using TaskFlow.Core.Requests;

// Define la lógica de negocio relacionada con la asignación de etiquetas a las tareas.

namespace TaskFlow.Core.Services
{
    public interface ITareaEtiquetaService
    {
        // Métodos GET
        Task<Result<IEnumerable<TareaEtiqueta>>> GetEtiquetasDeUnaTareaAsync(int tareaId);

        // Misc.
        // Comprobamos si una etiqueta ya existe con un nombre y color que recibimos de la tarea. Si existe, devolvemos esa etiqueta.
        // Si no existe, la creamos.
        Task<Result> ComprobarSiEtiquetaExisteOCrearASync(IEnumerable<NuevaEtiqueta> etiquetas);

        // Asignar etiquetas a una tarea concreta
        Task<Result> AsignarEtiquetaATareaASync(
            Tarea tarea,
            IEnumerable<NuevaEtiqueta> etiquetas
        );
    }
}