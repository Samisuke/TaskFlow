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
        public async Task <Result<IEnumerable<Comentario>>> GetComentariosDeUnUsuarioAsync(int idUsuario)
        {
            // Comprobar que el usuario existe
            var usuario = await _repoUsuario.ObtenerUsuarioPorIdAsync(idUsuario);
            if (usuario is null) return Result<IEnumerable<Comentario>>.Mal("ERROR. No se encuentra el usuario");

            // Sacar los comentarios del usuario
            var comentarios = await _repoComentario.ObtenerComentariosDeUnUsuarioAsync(idUsuario);
            if (!comentarios.Any()) return Result<IEnumerable<Comentario>>.Mal("ERROR. El usuario no tiene comentarios.");

            return Result<IEnumerable<Comentario>>.Bien(comentarios);
        }

        public async Task <Result<IEnumerable<Comentario>>> GetComentariosDeUnaTareaAsync(int idTarea)
        {
            // Comprobar que la tarea existe


            // Sacar los comentarios de la tarea
            var comentarios = await _repoComentario.ObtenerComentariosDeUnaTareaAsync(idTarea);
            if (!comentarios.Any()) return Result<IEnumerable<Comentario>>.Mal("ERROR. No hay comentarios en esta tarea.");

            return Result<IEnumerable<Comentario>>.Bien(comentarios);
        }

        public async Task <Result<Comentario>> GetComentarioPorIdAsync(int id)
        {
            var comentario = await _repoComentario.ObtenerComentarioPorIdAsync(id);
            if (comentario is null) return Result<Comentario>.Mal("ERROR. Este comentario no existe.");

            return Result<Comentario>.Bien(comentario);
        }
        
        // Métodos POST
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