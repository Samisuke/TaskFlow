using TaskFlow.Core.Models;

// Define las operaciones de acceso y persistencia de las relaciones entre usuarios y proyectos.

namespace TaskFlow.Core.Repositories
{
    public interface IProyectoUsuarioRepository
    {
        // GET
        Task <IEnumerable<ProyectoUsuario>> ObtenerTodosUsuariosDeUnProyectoAsync(int idProyecto);
        Task <ProyectoUsuario?> ObtenerUnUsuarioDeUnProyectoAsync(int idProyecto, int idUsuario);

        // POST
        Task CrearUsuarioAsync(ProyectoUsuario usuario);

        // Misc.
        Task <bool> GuardarCambiosAsync();
    }
}