using TaskFlow.Core.Services;
using TaskFlow.Core.Common;
using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Core.Dto.Paginacion;
using System.Text.RegularExpressions;

namespace TaskFlow.Infrastructure.Services
{
    public class ComentarioService : IComentarioService
    {
        // Inyección de repositorios
        private readonly IComentarioRepository _repoComentario;
        private readonly ITareaRepository _repoTarea;
        private readonly IProyectoPermissionService _proyectoPermission;
        private readonly IComentarioPermissionService _comentarioPermission;
        private readonly IHistorialService _historialService;
        private readonly TaskFlowDbContext _context;


        public ComentarioService(
            IComentarioRepository repoComentario,
            ITareaRepository repoTarea,
            IProyectoPermissionService proyectoPermission,
            IComentarioPermissionService comentarioPermission,
            IHistorialService historialService,
            TaskFlowDbContext context
        )
        {
            _repoComentario = repoComentario;
            _repoTarea = repoTarea;
            _proyectoPermission = proyectoPermission;
            _comentarioPermission = comentarioPermission;
            _historialService = historialService;
            _context = context;
        }
        // Métodos GET
        // Obtener los comentarios que ha hecho un usuario. Útil para ver una lsita de comentarios que un usario ha hecho en tu proyecto.
        public async Task <Result<PaginatedResult<Comentario>>> GetComentariosDeUnUsuarioAsync(
            int usuarioId,
            int pagina,
            int tamanoPagina
        )
        {
            var resultado = await _repoComentario.ObtenerComentariosDeUnUsuarioAsync(usuarioId, pagina, tamanoPagina);
            if (!resultado.Comentarios.Any()) return Result<PaginatedResult<Comentario>>.Mal("El usuario no tiene comentarios.");

            var totalPaginas = (int)Math.Ceiling(resultado.TotalItems / (double)tamanoPagina);

            var paginado = new PaginatedResult<Comentario>
            {
                Items = resultado.Comentarios,
                Pagina = pagina,
                TotalPaginas = totalPaginas,
                TotalItems = resultado.TotalItems,
                TamanoPagina = tamanoPagina
            };

            return Result<PaginatedResult<Comentario>>.Bien(paginado);
        }

        // Obtener todos los comentarios de una tarea. Útil para poner una sección de comentarios.
        public async Task <Result<PaginatedResult<Comentario>>> GetComentariosDeUnaTareaAsync(
            int tareaId,
            int pagina,
            int tamanoPagina
        )
        {
            var resultado = await _repoComentario.ObtenerComentariosDeUnaTareaAsync(tareaId, pagina, tamanoPagina);
            if (!resultado.Comentarios.Any()) return Result<PaginatedResult<Comentario>>.Mal("No hay comentarios en esta tarea.");

            var totalPaginas = (int)Math.Ceiling(resultado.TotalItems / (double)tamanoPagina);

            var paginado = new PaginatedResult<Comentario>
            {
                Items = resultado.Comentarios,
                Pagina = pagina,
                TotalPaginas = totalPaginas,
                TotalItems = resultado.TotalItems,
                TamanoPagina = tamanoPagina
            };

            return Result<PaginatedResult<Comentario>>.Bien(paginado);
        }

        // Obtener un comentario concreto. Útil si quieres poder entrar en el comentario.
        public async Task <Result<Comentario>> GetComentarioPorIdAsync(int comentarioId)
        {
            var comentario = await _repoComentario.ObtenerComentarioPorIdAsync(comentarioId);
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

            // Transacción
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
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
                await _historialService.RegistrarComentarioAsync(comentario, usuarioId);
                await _context.SaveChangesAsync();

                // Commit
                await transaction.CommitAsync();

                return Result<Comentario>.Bien(comentario);              
            }

            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Métodos PATCH
        // Modificar el contenido de un comentario.
        public async Task <Result<Comentario>> PatchComentarioAsync(
            int propiaId, 
            int comentarioId, 
            string? contenidoComentario
        )
        {
            int numeroCambios = 0;

            // Comprobaciones de existencia.
            var comentario = await _repoComentario.ObtenerComentarioPorIdAsync(comentarioId);
            if (comentario is null) return Result<Comentario>.Mal("No existe el comentario que quieres modificar.");
            if (comentario.Tarea is null) return Result<Comentario>.Mal("No existe la tarea del comentario que quieres modificar.");

            // Comprobaciones.
            if (!await _comentarioPermission.PuedeCambiarComentarioAsync(propiaId, comentario)) return Result<Comentario>.Mal("No puedes modificar este comentario.");

            // Transacción
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Realización de cambios.
                if(contenidoComentario is not null)
                {
                    comentario.Contenido = contenidoComentario;
                    numeroCambios += 1;
                }

                // Base de datos.
                if (numeroCambios == 0) return Result<Comentario>.Mal("No se han detectado cambios.");
                await _historialService.ModificarComentarioAsync(comentario, propiaId);
                await _context.SaveChangesAsync();

                // Commit
                await transaction.CommitAsync();

                return Result<Comentario>.Bien(comentario);              
            }

            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}