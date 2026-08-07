using TaskFlow.Core.Repositories;
using TaskFlow.Core.Common;
using TaskFlow.Core.Models;

// Define la lógica de negocio relacionada con los usuarios.

namespace TaskFlow.Core.Services
{
    public interface IUsuarioService
    {
        // Métodos GET
        Task<Result<IEnumerable<Usuario>>> GetTodosUsuariosAsync();
        Task<Result<Usuario>> GetUsuarioPorIdAsync(int usuarioId);
        Task<Result<Usuario>> GetUsuarioPorEmailAsync(string emailUsuario);
        
        // Métodos POST
        Task<Result<Usuario>> PostUsuarioAsync(
            string nombreUsuario,
            string apellidosUsuario,
            string emailUsuario,
            string passUsuario
        );

        // Métodos PATCH
        Task<Result<Usuario>> PatchUsuarioAsync(
            int usuarioId,
            string? nombreUsuario,
            string? apellidosUsuario,
            string? emailUsuario
        );
        Task<Result<Usuario>> PatchUsuarioPassAsync(int usuarioId, string passNueva, string passAntigua);

    
    }
}

