using TaskFlow.Core.Models;

// Define las operaciones de acceso y persistencia de datos relacionadas con los comentarios.

namespace TaskFlow.Core.Repositories
{
    public interface IComentarioRepository
    {
        // GET
        Task <IEnumerable<Comentario>> ObtenerComentariosDeUnaTareaAsync(int tareaId);
        Task <IEnumerable<Comentario>> ObtenerComentariosDeUnUsuarioAsync(int usuarioId);
        Task <Comentario?> ObtenerComentarioPorIdAsync(int comentarioId);

        // POST
        Task CrearComentarioAsync(Comentario comentario);

        // Misc.
        Task <bool> GuardarCambiosAsync();
    }
}