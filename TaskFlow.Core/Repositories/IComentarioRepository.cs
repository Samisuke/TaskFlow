using TaskFlow.Core.Models;

namespace TaskFlow.Core.Repositories
{
    public interface IComentarioRepository
    {
        // GET
        Task <IEnumerable<Comentario>> ObtenerComentariosDeUnaTareaAsync(int idTarea);
        Task <IEnumerable<Comentario>> GetComentariosPropiosAsync(int idPropia);
        Task <IEnumerable<Comentario>> ObtenerComentariosDeUnUsuarioAsync(int idUsuario);
        Task <Comentario?> ObtenerComentarioPorIdAsync(int id);

        // POST
        Task CrearComentarioAsync(Comentario comentario);

        // Misc.
        Task <bool> GuardarCambiosAsync();
    }
}