using TaskFlow.Core.Services;
using TaskFlow.Core.Common;
using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Services;

namespace TaskFlow.Infrastructure.Services
{
    public class ComentarioService : IComentarioService
    {
        // Inyección de repositorios
        private readonly IComentarioRepository _repoComentario;
        private readonly ITareaRepository _repoTarea;
        private readonly IProyectoPermissionService _proyectoPermission;
        private readonly IComentarioPermissionService _comentarioPermission;
        private readonly IHIstorialService _historialService;

        public ComentarioService(
            IComentarioRepository repoComentario,
            ITareaRepository repoTarea,
            IProyectoPermissionService proyectoPermission,
            IComentarioPermissionService comentarioPermission,
            IHIstorialService historialService
        )
        {
            _repoComentario = repoComentario;
            _repoTarea = repoTarea;
            _proyectoPermission = proyectoPermission;
            _comentarioPermission = comentarioPermission;
            _historialService = historialService;
        }
        // Métodos GET
        // Obtener los comentarios que ha hecho un usuario. Útil para ver una lsita de comentarios que un usario ha hecho en tu proyecto.
        public async Task <Result<IEnumerable<Comentario>>> GetComentariosDeUnUsuarioAsync(int idUsuario)
        {
            var comentarios = await _repoComentario.ObtenerComentariosDeUnUsuarioAsync(idUsuario);
            if (!comentarios.Any()) return Result<IEnumerable<Comentario>>.Mal("El usuario no tiene comentarios.");

            return Result<IEnumerable<Comentario>>.Bien(comentarios);
        }

        // Obtener los comentarios que has hecho tú mismo. Útil para ver una lista de tus propios comentarios en las diferentes tareas.
        public async Task <Result<IEnumerable<Comentario>>> GetComentariosPropiosAsync(int idPropia)
        {
            var comentarios = await _repoComentario.ObtenerComentariosDeUnUsuarioAsync(idPropia);
            if (!comentarios.Any()) return Result<IEnumerable<Comentario>>.Mal("No tienes comentarios todavía.");

            return Result<IEnumerable<Comentario>>.Bien(comentarios);
        }

        // Obtener todos los comentarios de una tarea. Útil para poner una sección de comentarios.
        public async Task <Result<IEnumerable<Comentario>>> GetComentariosDeUnaTareaAsync(int idTarea)
        {
            var comentarios = await _repoComentario.ObtenerComentariosDeUnaTareaAsync(idTarea);
            if (!comentarios.Any()) return Result<IEnumerable<Comentario>>.Mal("No hay comentarios en esta tarea.");

            return Result<IEnumerable<Comentario>>.Bien(comentarios);
        }

        // Obtener un comentario concreto. Útil si quieres poder entrar en el comentario.
        public async Task <Result<Comentario>> GetComentarioPorIdAsync(int id)
        {
            var comentario = await _repoComentario.ObtenerComentarioPorIdAsync(id);
            if (comentario is null) return Result<Comentario>.Mal("Este comentario no existe.");

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
            // Comprobaciones de existencia.
            var tarea = await _repoTarea.ObtenerTareaPorIdAsync(tareaId);
            if (tarea is null) return Result<Comentario>.Mal("La tarea a la que quieres añadir el comentario no existe.");

            // Comprobaciones de proyecto.
            if (!await _proyectoPermission.EsMiembroActivoAsync(tarea.ProyectoId, usuarioId)) return Result<Comentario>.Mal("No puedes comentar en esta tarea.");

            // Creación de comentario.
            var comentario = new Comentario
            {
                Contenido = contenidoComentario,
                Fecha = DateTime.UtcNow,
                UsuarioId = usuarioId,
                TareaId = tareaId
            };

            // Base de datos.
            await _repoComentario.CrearComentarioAsync(comentario);
            var guardadoExitoso = await _repoComentario.GuardarCambiosAsync();
            if (!guardadoExitoso) return Result<Comentario>.Mal("Fallo inesperado al guardar el comentario. Inténtalo de nuevo más tarde.");

            // Registrar historial.
            await _historialService.RegistrarComentarioAsync(comentario, usuarioId);

            return Result<Comentario>.Bien(comentario);
        }

        // Métodos PATCH
        // Modificar el contenido de un comentario.
        public async Task <Result<Comentario>> PatchComentarioAsync(int idPropia, int idComentario, string contenidoComentario)
        {
            int numeroCambios = 0;
            var comentario = await _repoComentario.ObtenerComentarioPorIdAsync(idComentario);
            if (comentario is null) return Result<Comentario>.Mal("No existe el comentario que quieres modificar.");
            if (comentario.Tarea is null) return Result<Comentario>.Mal("No existe la tarea del comentario que quieres modificar.");

            // Comprobaciones.
            if (!await _comentarioPermission.PuedeCambiarComentarioAsync(idPropia, comentario)) return Result<Comentario>.Mal("No puedes modificar este comentario.");

            // Realización de cambios.
            if(contenidoComentario is not null)
            {
                comentario.Contenido = contenidoComentario;
                numeroCambios += 1;
            }
            
            // Base de datos.
            if (numeroCambios == 0) return Result<Comentario>.Mal("ERROR. No se han detectado cambios.");
            var guardadoExitoso = await _repoComentario.GuardarCambiosAsync();
            if(!guardadoExitoso) return Result<Comentario>.Mal("Fallo inesperado al guardar el comentario. Inténtalo de nuevo más tarde.");

            // Registrar historial.
            await _historialService.ModificarComentarioAsync(comentario, idPropia);
            
            return Result<Comentario>.Bien(comentario);
        }
    }
}