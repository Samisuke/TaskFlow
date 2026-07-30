using TaskFlow.Core.Models;

namespace Taskflow.Core.Repositories
{
    public interface IProyectoUsuarioRepository
    {
        // Peticiones GET
        Task <IEnumerable<ProyectoUsuario>> ObtenerTodosUsuariosDeUnProyectoAsync(int idProyecto);
        Task <ProyectoUsuario?> ObtenerUnUsuarioDeUnProyectoAsync(int idProyecto, int idUsuario);

        // Peticiones POST
        Task CrearUsuarioAsync(ProyectoUsuario usuario);
        Task <bool> GuardarCambiosAsync();
    }
}