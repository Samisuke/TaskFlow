using TaskFlow.Core.Models;

// Define las operaciones de acceso y persistencia de las relaciones entre usuarios y proyectos.

namespace TaskFlow.Core.Repositories
{
    public interface IProyectoUsuarioRepository
    {
        // GET
        Task <IEnumerable<ProyectoUsuario>> ObtenerTodosUsuariosDeUnProyectoAsync(int proyectoId);
        Task <ProyectoUsuario?> ObtenerUnUsuarioDeUnProyectoAsync(int proyectoId, int usuarioId);

        // POST
        Task CrearUsuarioAsync(ProyectoUsuario usuario);
    }
}