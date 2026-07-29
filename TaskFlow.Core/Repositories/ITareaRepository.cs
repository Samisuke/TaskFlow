using TaskFlow.Core.Models;

namespace TaskFlow.Core.Repositories
{
    public interface ITareaRepository
    {
        // Obtención de Tareas
        Task <IEnumerable<Tarea>> ObtenerTareasPendientesPorUsuarioIdAsync(int idUsuario);
        Task <IEnumerable<Tarea>> ObtenerTareasDadasPorUsuarioIdAsync(int idUsuario);
        Task <IEnumerable<Tarea>> ObtenerTareasDeUnProyectoAsync(int idProyecto);
        Task <Tarea?> ObtenerTareaPorIdAsync(int idTarea);

        // Misc.
        Task CrearTareaAsync(Tarea tarea);
        Task<bool> GuardarCambiosAsync();
    }
}