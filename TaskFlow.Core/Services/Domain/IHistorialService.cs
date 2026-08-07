using TaskFlow.Core.Common;
using TaskFlow.Core.Models;

// Define las operaciones necesarias para registrar y consultar el historial de cambios de los proyectos.

namespace TaskFlow.Core.Services
{
    public interface IHistorialService
    {
        // Métodos GET
        Task<Result<IEnumerable<Historial>>> GetHistorialAsync(int idTarea);

        // Registrar un comentario
        Task RegistrarComentarioAsync(Comentario comentario, int idPropia);

        // Modificar un comentario
        Task ModificarComentarioAsync(Comentario comentario, int idPropia);

        // Modificar un proyecto
        Task ModificarProyectoAsync(Proyecto proyecto, int idPropia);

        // Modificar dueño de un proyecto
        Task ModificarDueñoProyectoAsync(Proyecto proyecto, int idPropia);

        // Añadir persona a un proyecto
        Task AñadirPersonaProyectoAsync(int proyectoId, int idPropia);

        // Modificar persona de un proyecto
        Task ModificarPersonaProyectoAsync(int proyectoId, int idPropia);

        // Registrar una tarea
        Task RegistrarTareaAsync(Tarea tarea);

        // Modificar una tarea
        Task ModificarTareaAsync(Tarea tarea, int idPropio);

        // Modificar el estado de una  tarea
        Task ModificarEstadoTareaAsync(Tarea tarea, int idPropio);
    }
}