using TaskFlow.Core.Common;
using TaskFlow.Core.Enums;
using TaskFlow.Core.Models;

namespace TaskFlow.Core.Services
{
    public interface IProyectoUsuarioService
    {
        // Métodos GET 
        Task<Result<IEnumerable<ProyectoUsuario>>> GetTodosLosUsuariosDeUnProyectoAsync(int idProyecto);
        Task<Result<ProyectoUsuario?>> GetUsuarioDeUnProyectoAsync(int idProyecto, int idUsuario);

        // Métodos POST
        Task<Result<ProyectoUsuario>> PostUsuarioAsync(
            int idPropia,
            int usuarioId,
            int proyectoId,
            // El usuario estará activo por defecto cuando lo añadas a un proyecto.
            RolProyecto rolUsuario
        );

        // Métodos PATCH
        Task<Result<ProyectoUsuario>> PatchUsuarioAsync(
            int idPropia,
            int idUsuarioACambiar,
            int idProyecto,
            bool? activoUsuario,
            RolProyecto? rolUsuario
        );
    }
}