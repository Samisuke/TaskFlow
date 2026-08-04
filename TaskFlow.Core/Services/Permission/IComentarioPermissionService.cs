using TaskFlow.Core.Models;

namespace TaskFlow.Core.Services
{
    public interface IComentarioPermissionService
    {
        // Comprueba que pertenezcas al proyecto, estés activo y seas el dueño de un comentario
        Task<bool> PuedeCambiarComentarioAsync(int idPropia, Comentario comentario);
    }
}