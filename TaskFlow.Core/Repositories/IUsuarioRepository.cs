using TaskFlow.Core.Models;

namespace TaskFlow.Core.Repositories
{
    public interface IUsuarioRepository
    {   
        // Obtención de usuarios
        Task <IEnumerable<Usuario>> ObtenerTodosUsuariosAsync();
        Task <Usuario?> ObtenerUsuarioPorIdAsync(int idUsuario);
        Task <Usuario?> ObtenerUsuarioPorEmailAsync(string email);

        // Misc.
        Task CrearUnUsuarioNuevoAsync(Usuario usuario);
        Task <bool> GuardarCambiosAsync();
    }
}