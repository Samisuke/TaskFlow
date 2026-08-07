using TaskFlow.Core.Models;

// Define las operaciones de acceso y persistencia de datos relacionadas con las tareas.

namespace TaskFlow.Core.Repositories
{
    public interface ITareaRepository
    {
        // GET
        Task <IEnumerable<Tarea>> ObtenerTareasPendientesPorUsuarioIdAsync(int usuarioId);
        Task <IEnumerable<Tarea>> ObtenerTareasDadasPorUsuarioIdAsync(int usuarioId);
        Task <IEnumerable<Tarea>> ObtenerTareasDeUnProyectoAsync(int proyectoId);
        Task <Tarea?> ObtenerTareaPorIdAsync(int tareaId);

        // POST
        Task CrearTareaAsync(Tarea tarea);

        // Misc.
        Task<bool> GuardarCambiosAsync();
    }
}