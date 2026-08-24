using TaskFlow.Core.Common;
using TaskFlow.Core.Dto.Paginacion;
using TaskFlow.Core.Models;

// Define la lógica de negocio relacionada con los comentarios.

namespace TaskFlow.Core.Services
{
    public interface IComentarioService
    {
        // Métodos GET
        Task <Result<PaginatedResult<Comentario>>> GetComentariosDeUnUsuarioAsync(
            int usuarioId,
            int pagina,
            int tamanoPagina
        );
        Task <Result<PaginatedResult<Comentario>>> GetComentariosDeUnaTareaAsync(
            int tareaId,
            int pagina,
            int tamanoPagina
        );
        Task <Result<Comentario>> GetComentarioPorIdAsync(int comentarioId);
        
        // Métodos POST
        Task <Result<Comentario>> PostComentarioAsync(
            string contenidoComentario,
            int usuarioId,
            int tareaId
        );

        // Métodos PATCH
        Task <Result<Comentario>> PatchComentarioAsync(int propioId, int comentarioId, string contenidoComentario);
    }
}