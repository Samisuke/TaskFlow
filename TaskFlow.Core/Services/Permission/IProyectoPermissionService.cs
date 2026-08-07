using TaskFlow.Core.Models;

// Define las comprobaciones de permisos necesarias para operar sobre proyectos.

namespace TaskFlow.Core.Services
{
    public interface IProyectoPermissionService
    {
        // Comprueba que pertenezcas al proyecto y estés activo para poder comentar en el.
        Task<bool> EsMiembroActivoAsync(int proyectoId, int propiaId);

        // Contiene las comprobaciones para poder modificar un proyecto.
        Task<bool>PuedeModificarProyectoAsync(int proyectoId, int propiaId);

        // Contiene las comprobaciones para poder transferir un proyecto.
        Task<bool> PuedesTransferirProyectoAsync(Proyecto proyecto, Usuario usuarioNuevo, int propiaId);

        // Contiene las comprobaciones para poder añadir un usuario a un proyecto
        Task<bool> PuedeAñadirPersonasAsync(int proyectoId, int nuevoUsuarioId, int propiaId);
    }

}