using TaskFlow.Core.Models;

// Define las operaciones de acceso y persistencia de datos relacionadas con los comentarios.

namespace TaskFlow.Core.Repositories
{
    public interface IComentarioRepository
    {
        // GET
        Task <(IEnumerable<Comentario> Comentarios, int TotalItems)> ObtenerComentariosDeUnaTareaAsync(
            int tareaId,
            int pagina,
            int tamanoPagina
        );
        Task <(IEnumerable<Comentario> Comentarios, int TotalItems)> ObtenerComentariosDeUnUsuarioAsync(
            int usuarioId,
            int pagina,
            int tamanoPagina
        );
        Task <Comentario?> ObtenerComentarioPorIdAsync(int comentarioId);

        // POST
        Task CrearComentarioAsync(Comentario comentario);
    }
}