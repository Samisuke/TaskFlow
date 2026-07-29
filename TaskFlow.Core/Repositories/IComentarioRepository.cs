using TaskFlow.Core.Models;

namespace TaskFlow.Core.Repositories
{
    public interface IComentarioRepository
    {
        // Obtención de Comentarios
        Task <IEnumerable<Comentario>> ObtenerComentariosDeUnaTareaAsync(int idTarea);
        Task <IEnumerable<Comentario>> ObtenerComentariosDeUnUsuarioAsync(int idUsuario);
        Task <Comentario?> ObtenerComentarioPorIdAsync(int id);

        //Misc.
        Task CrearComentarioAsync(Comentario comentario);
        Task <bool> GuardarCambiosAsync();
    }
}