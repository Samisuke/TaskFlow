using TaskFlow.Core.Common;
using TaskFlow.Core.Models;

// Define las operaciones necesarias para registrar y consultar el historial de cambios de los proyectos.

namespace TaskFlow.Core.Services
{
    public interface IHistorialService
    {
        // Métodos GET
        Task<Result<IEnumerable<Historial>>> GetHistorialAsync(int tareaId);

        // Registrar un comentario
        Task RegistrarComentarioAsync(Comentario comentario, int propiaId);

        // Modificar un comentario
        Task ModificarComentarioAsync(Comentario comentario, int propiaId);

        // Modificar un proyecto
        Task ModificarProyectoAsync(Proyecto proyecto, int propiaId);

        // Modificar dueño de un proyecto
        Task ModificarDueñoProyectoAsync(Proyecto proyecto, int propiaId);

        // Añadir persona a un proyecto
        Task AñadirPersonaProyectoAsync(int proyectoId, int propiaId);

        // Modificar persona de un proyecto
        Task ModificarPersonaProyectoAsync(int proyectoId, int propiaId);

        // Registrar una tarea
        Task RegistrarTareaAsync(Tarea tarea);

        // Modificar una tarea
        Task ModificarTareaAsync(Tarea tarea, int propiaId);

        // Modificar el estado de una  tarea
        Task ModificarEstadoTareaAsync(Tarea tarea, int propiaId);
    }
}