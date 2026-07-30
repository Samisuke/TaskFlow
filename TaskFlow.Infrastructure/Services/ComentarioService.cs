using TaskFlow.Core.Services;
using TaskFlow.Core.Common;
using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;

namespace TaskFlow.Infrastructure.Services
{
    public class ComentarioService : IComentarioService
    {
        // Inyección de repositorios
        private readonly IComentarioRepository _repoComentario;
        private readonly IUsuarioRepository _repoUsuario;
        public ComentarioService(IComentarioRepository repoComentario, IUsuarioRepository repoUsuario)
        {
            _repoComentario = repoComentario;
            _repoUsuario = repoUsuario;
        }
        // Métodos GET
        // Obtener los comentarios que ha hecho un usuario. Útil para ver una lsita de tus propios comentarios en las diferentes tareas.
        public async Task <Result<IEnumerable<Comentario>>> GetComentariosDeUnUsuarioAsync(int idUsuario)
        {
            var comentarios = await _repoComentario.ObtenerComentariosDeUnUsuarioAsync(idUsuario);
            if (!comentarios.Any()) return Result<IEnumerable<Comentario>>.Mal("ERROR. El usuario no tiene comentarios.");

            return Result<IEnumerable<Comentario>>.Bien(comentarios);
        }

        // Obtener todos los comentarios de una tarea. Útil para poner una sección de comentarios.
        public async Task <Result<IEnumerable<Comentario>>> GetComentariosDeUnaTareaAsync(int idTarea)
        {
            var comentarios = await _repoComentario.ObtenerComentariosDeUnaTareaAsync(idTarea);
            if (!comentarios.Any()) return Result<IEnumerable<Comentario>>.Mal("ERROR. No hay comentarios en esta tarea.");

            return Result<IEnumerable<Comentario>>.Bien(comentarios);
        }

        // Obtener un comentario concreto. Útil si quieres poder entrar en el comentario.
        public async Task <Result<Comentario>> GetComentarioPorIdAsync(int id)
        {
            var comentario = await _repoComentario.ObtenerComentarioPorIdAsync(id);
            if (comentario is null) return Result<Comentario>.Mal("ERROR. Este comentario no existe.");

            return Result<Comentario>.Bien(comentario);
        }
        
        // Métodos POST
        // Crear un comentario en una tarea.
        public async Task <Result<Comentario>> PostComentarioAsync(
        string contenidoComentario,
        int usuarioId,
        int tareaId
        )
        {
            var comentario = new Comentario
            {
                Contenido = contenidoComentario,
                Fecha = DateTime.UtcNow,
                UsuarioId = usuarioId,
                TareaId = tareaId
            };

            var guardadoExitoso = await _repoComentario.GuardarCambiosAsync();
            if (!guardadoExitoso) return Result<Comentario>.Mal("ERROR. Fallo inesperado al guardar el comentario. Inténtalo de nuevo más tarde.");

            return Result<Comentario>.Bien(comentario);
        }

        // Métodos PATCH
        // Modificar el contenido de un comentario.
        public async Task <Result<Comentario>> PatchComentarioAsync(int id, string contenidoComentario)
        {
            int numeroCambios = 0;
            var comentario = await _repoComentario.ObtenerComentarioPorIdAsync(id);
            if (comentario is null) return Result<Comentario>.Mal("ERROR. El comentario no existe.");

            if(contenidoComentario is not null)
            {
                comentario.Contenido = contenidoComentario;
                numeroCambios += 1;
            }
            
            if (numeroCambios == 0) return Result<Comentario>.Mal("ERROR. No se han detectado cambios.");
            var guardadoExitoso = await _repoComentario.GuardarCambiosAsync();
            if(!guardadoExitoso) return Result<Comentario>.Mal("ERROR. Fallo inesperado al guardar el comentario. Inténtalo de nuevo más tarde.");

            return Result<Comentario>.Bien(comentario);
        }
    }
}