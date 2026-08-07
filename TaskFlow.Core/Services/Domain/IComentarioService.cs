using TaskFlow.Core.Common;
using TaskFlow.Core.Models;

// Define la lógica de negocio relacionada con los comentarios.

namespace TaskFlow.Core.Services
{
    public interface IComentarioService
    {
        // Métodos GET
        Task <Result<IEnumerable<Comentario>>> GetComentariosDeUnUsuarioAsync(int usuarioId);
        Task <Result<IEnumerable<Comentario>>> GetComentariosDeUnaTareaAsync(int tareaId);
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