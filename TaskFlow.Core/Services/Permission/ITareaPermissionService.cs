using TaskFlow.Core.Models;

// Define las comprobaciones de permisos necesarias para operar sobre tareas.

namespace TaskFlow.Core.Services
{
    public interface ITareaPermissionService
    {
        // Comprueba que pertenezcas al proyecto, estés activo y tu rol.
        Task<bool> PuedePublicarTareasAsync(int proyectoId, int propiaId);

        // Contiene las comprobaciones para poder modificar una tarea.
        Task<bool>PuedeModificarTareasAsync(int propiaId, Tarea tarea);

        // Contiene las comprobaciones para poder modificar el estado de una tarea.
        Task<bool> PuedeModificarEstadoTareaAsync(int propiaId, Tarea tarea);
    }

}