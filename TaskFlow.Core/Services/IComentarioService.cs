using TaskFlow.Core.Common;
using TaskFlow.Core.Models;

namespace TaskFlow.Core.Services
{
    public interface IComentarioService
    {
        // Métodos GET
        Task <Result<IEnumerable<Comentario>>> GetComentariosDeUnUsuarioAsync(int idUsuario);
        Task <Result<IEnumerable<Comentario>>> GetComentariosDeUnaTareaAsync(int idTarea);
        Task <Result<Comentario>> GetComentarioPorIdAsync(int id);
        
        // Métodos POST
        Task <Result<Comentario>> PostComentarioAsync(
            string contenidoComentario,
            int usuarioId,
            int tareaId);
        // Métodos PATCH
        Task <Result<Comentario>> PatchComentarioAsync(int id, string contenidoComentario);
    }
}