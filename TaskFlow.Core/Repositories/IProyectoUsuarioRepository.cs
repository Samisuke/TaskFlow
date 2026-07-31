using TaskFlow.Core.Models;

namespace Taskflow.Core.Repositories
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