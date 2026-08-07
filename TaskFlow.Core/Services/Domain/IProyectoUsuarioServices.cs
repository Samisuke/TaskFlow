using TaskFlow.Core.Common;
using TaskFlow.Core.Enums;
using TaskFlow.Core.Models;

// Define la lógica de negocio relacionada con la pertenencia de usuarios a proyectos.

namespace TaskFlow.Core.Services
{
    public interface IProyectoUsuarioService
    {
        // Métodos GET 
        Task<Result<IEnumerable<ProyectoUsuario>>> GetTodosLosUsuariosDeUnProyectoAsync(int proyectoId);
        Task<Result<ProyectoUsuario?>> GetUsuarioDeUnProyectoAsync(int proyectoId, int usuarioId);

        // Métodos POST
        Task<Result<ProyectoUsuario>> PostUsuarioAsync(
            int propiaId,
            int usuarioId,
            int proyectoId,
            // El usuario estará activo por defecto cuando lo añadas a un proyecto.
            RolProyecto rolUsuario
        );

        // Métodos PATCH
        Task<Result<ProyectoUsuario>> PatchUsuarioAsync(
            int propiaId,
            int idUsuarioACambiar,
            int proyectoId,
            bool? activoUsuario,
            RolProyecto? rolUsuario
        );
    }
}