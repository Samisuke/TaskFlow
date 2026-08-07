using TaskFlow.Core.Common;
using TaskFlow.Core.Models;
using TaskFlow.Core.Enums;
using TaskFlow.Core.Requests;

// Define la lógica de negocio relacionada con las tareas.

namespace TaskFlow.Core.Services
{
    public interface ITareaService
    {
        // Métodos GET
        Task <Result<IEnumerable<Tarea>>> GetTareasPendientesDeUsuarioAsync(int usuarioId);
        Task <Result<IEnumerable<Tarea>>> GetTareasDadasDeUsuarioAsync(int usuarioId);
        Task <Result<IEnumerable<Tarea>>> GetTareasDeUnProyectoAsync(int usuarioId);
        Task <Result<Tarea?>> GetTareaPorIdAsync(int tareaId);

        // Métodos POST
        Task <Result<Tarea>> PostTareaAsync(
            int propiaId,
            string tituloTarea,
            string descripcionTarea,
            EstadoTarea estadoTareaTarea,
            PrioridadTarea prioridadTareaTarea,
            DateTimeOffset fechaLimiteTarea,
            int proyectoId,
            int asignadoId,
            List<NuevaEtiqueta> etiquetas
        );

        // Métodos PATCH
        Task <Result<Tarea>> PatchTareaAsync(
            int propiaId,
            int tareaId,
            string? tituloTarea,
            string? descripcionTarea,
            PrioridadTarea? prioridadTareaTarea,
            DateTimeOffset? fechaLimiteTarea,
            List<NuevaEtiqueta> etiquetas
        );

        Task <Result<Tarea>> PatchEstadoTareaAsync(
            int propiaId, 
            int tareaId,
            EstadoTarea estadoTareaTarea
        );
    }
}