using TaskFlow.Core.Models;

namespace Taskflow.Core.Services
{
    public interface ITareaPermissionService
    {
        // Comprueba que pertenezcas al proyecto, estés activo y tu rol.
        Task<bool> PuedePublicarTareasAsync(int idProyecto, int idPropia);

        // Contiene las comprobaciones para poder modificar una tarea.
        Task<bool>PuedeModificarTareasAsync(int idPropia, Tarea tarea);

        // Contiene las comprobaciones para poder modificar el estado de una tarea.
        Task<bool> PuedeModificarEstadoTareaAsync(int idPropia, Tarea tarea);
    }

}