using TaskFlow.Core.Models;

// Define las comprobaciones de permisos necesarias para operar sobre comentarios.

namespace TaskFlow.Core.Services
{
    public interface IComentarioPermissionService
    {
        // Comprueba que pertenezcas al proyecto, estés activo y seas el dueño de un comentario
        Task<bool> PuedeCambiarComentarioAsync(int propiaId, Comentario comentario);
    }
}