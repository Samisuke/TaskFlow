using TaskFlow.Core.Models;

// Define las operaciones de acceso y persistencia de datos relacionadas con las tareas.

namespace TaskFlow.Core.Repositories
{
    public interface ITareaRepository
    {
        // GET
        Task <(IEnumerable<Tarea> Tareas, int TotalITems)> ObtenerTareasPendientesPorUsuarioIdAsync(
            int usuarioId,
            int pagina,
            int tamanoPagina
        );
        Task <(IEnumerable<Tarea> Tareas, int TotalITems)> ObtenerTareasDadasPorUsuarioIdAsync(
            int usuarioId,
            int pagina,
            int tamanoPagina
        );
        Task <(IEnumerable<Tarea> Tareas, int TotalITems)> ObtenerTareasDeUnProyectoAsync(
            int proyectoId,
            int pagina,
            int tamanoPagina
        );
        Task <Tarea?> ObtenerTareaPorIdAsync(int tareaId);

        // POST
        Task CrearTareaAsync(Tarea tarea);
        
    }
}