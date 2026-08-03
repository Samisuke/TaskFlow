using TaskFlow.Core.Common;
using TaskFlow.Core.Models;
using TaskFlow.Core.Enums;
using TaskFlow.Core.Requests;

namespace TaskFlow.Core.Services
{
    public interface ITareaService
    {
        // Métodos GET
        Task <Result<IEnumerable<Tarea>>> GetTareasPendientesDeUsuarioAsync(int idUsuario);
        Task <Result<IEnumerable<Tarea>>> GetTareasDadasDeUsuarioAsync(int idUsuario);
        Task <Result<IEnumerable<Tarea>>> GetTareasDeUnProyectoAsync(int idProyecto);
        Task <Result<Tarea?>> GetTareaPorIdAsync(int id);

        // Métodos POST
        Task <Result<Tarea>> PostTareaAsync(
            int idPropia,
            string tituloTarea,
            string descripcionTarea,
            EstadoTarea estadoTareaTarea,
            PrioridadTarea prioridadTareaTarea,
            DateTimeOffset fechaLimiteTarea,
            int proyectoId,
            int asignadoId,
            IEnumerable<NuevaEtiqueta> etiquetas
        );

        // Métodos PATCH
        Task <Result<Tarea>> PatchTareaAsync(
            int idPropia,
            int idTarea,
            string? tituloTarea,
            string? descripcionTarea,
            PrioridadTarea? prioridadTareaTarea,
            DateTimeOffset? fechaLimiteTarea,
            IEnumerable<NuevaEtiqueta> etiquetas
        );

        Task <Result<Tarea>> PatchEstadoTareaAsync(
            int idPropia, 
            int idTarea,
            EstadoTarea estadoTareaTarea
        );
    }
}