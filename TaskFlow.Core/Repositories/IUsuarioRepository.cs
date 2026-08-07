using TaskFlow.Core.Models;

// Define las operaciones de acceso y persistencia de datos relacionadas con los usuarios.

namespace TaskFlow.Core.Repositories
{
    public interface IUsuarioRepository
    {   
        // GET
        // Metodo implementado para la administración de la aplicación.
        Task <IEnumerable<Usuario>> ObtenerTodosUsuariosAsync();
        Task <Usuario?> ObtenerUsuarioPorIdAsync(int usuarioId);
        Task <Usuario?> ObtenerUsuarioPorEmailAsync(string emailUsuario);

        // POST
        Task CrearUnUsuarioNuevoAsync(Usuario usuario);
        
        // Misc.
        Task <bool> GuardarCambiosAsync();
    }
}