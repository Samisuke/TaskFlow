using TaskFlow.Core.Models;

namespace Taskflow.Core.Services
{
    public interface IProyectoPermissionService
    {
        // Comprueba que pertenezcas al proyecto y estés activo para poder comentar en el.
        Task<bool> EsMiembroActivoAsync(int idProyecto, int idPropia);
        Task<bool> PuedesTransferirProyectoAsync(Proyecto proyecto, Usuario usuarioNuevo, int idPropia);
    }
}