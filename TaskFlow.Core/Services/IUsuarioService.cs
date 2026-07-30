using TaskFlow.Core.Repositories;
using TaskFlow.Core.Common;
using TaskFlow.Core.Models;

namespace TaskFlow.Core.Services
{
    public interface IUsuarioService
    {
        // Métodos GET
        Task<Result<IEnumerable<Usuario>>> GetTodosUsuariosAsync();
        Task<Result<Usuario>> GetUsuarioPorIdAsync(int idUsuario);
        Task<Result<Usuario>> GetUsuarioPorEmailAsync(string emailUsuario);
        
        // Métodos POST
        Task<Result<Usuario>> PostUsuarioAsync(
            string nombreUsuario,
            string apellidosUsuario,
            string emailUsuario,
            string passUsuario,
            bool Activo
        );

        // Métodos PATCH
        Task<Result<Usuario>> PatchUsuarioAsync(
            int idUsuario,
            string? nombreUsuario,
            string? apellidosUsuario,
            string? emailUsuario,
            bool? Activo
        );
        Task<Result<Usuario>> PatchUsuarioPassAsync(int idUsuario, string? passNueva);

    
    }
}

