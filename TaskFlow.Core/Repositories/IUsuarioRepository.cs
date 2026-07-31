using TaskFlow.Core.Models;

namespace TaskFlow.Core.Repositories
{
    public interface IUsuarioRepository
    {   
        // GET
        Task <IEnumerable<Usuario>> ObtenerTodosUsuariosAsync();
        Task <Usuario?> ObtenerUsuarioPorIdAsync(int idUsuario);
        Task <Usuario?> ObtenerUsuarioPorEmailAsync(string email);

        // POST
        Task CrearUnUsuarioNuevoAsync(Usuario usuario);
        
        // Misc.
        Task <bool> GuardarCambiosAsync();
    }
}