using TaskFlow.Core.Models;

namespace TaskFlow.Core.Repositories
{
    public interface ITareaRepository
    {
        // GET
        Task <IEnumerable<Tarea>> ObtenerTareasPendientesPorUsuarioIdAsync(int idUsuario);
        Task <IEnumerable<Tarea>> ObtenerTareasDadasPorUsuarioIdAsync(int idUsuario);
        Task <IEnumerable<Tarea>> ObtenerTareasDeUnProyectoAsync(int idProyecto);
        Task <Tarea?> ObtenerTareaPorIdAsync(int idTarea);

        // POST
        Task CrearTareaAsync(Tarea tarea);

        // Misc.
        Task<bool> GuardarCambiosAsync();
    }
}