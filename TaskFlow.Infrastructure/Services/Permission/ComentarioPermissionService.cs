using TaskFlow.Core.Repositories;
using TaskFlow.Core.Models;
using TaskFlow.Core.Services;

namespace TaskFlow.Infrastructure.Services
{
    public class ComentarioPermissionService : IComentarioPermissionService
    {
        // Inyección del repositorio
        private readonly IProyectoUsuarioRepository _repoProyectoUsuario;

        public ComentarioPermissionService(
            IProyectoUsuarioRepository repoPropyectoUsuario)
        {
            _repoProyectoUsuario = repoPropyectoUsuario;
        }

        // Comprueba que pertenezcas al proyecto, estés activo y seas el dueño de un comentario
        public async Task<bool> PuedeCambiarComentarioAsync(int idPropia, Comentario comentario)
        {
            var proyectoUsuario = await _repoProyectoUsuario.ObtenerUnUsuarioDeUnProyectoAsync(comentario.Tarea.ProyectoId, idPropia);

            return proyectoUsuario is not null
            && proyectoUsuario.Activo
            && comentario.UsuarioId == idPropia;
        }
    }
}