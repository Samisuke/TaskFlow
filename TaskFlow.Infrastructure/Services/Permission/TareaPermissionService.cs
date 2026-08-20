using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;


namespace TaskFlow.Core.Services
{
    public class TareaPermissionService : ITareaPermissionService
    {
        // Inyección de repositorios.
        private readonly IProyectoUsuarioRepository _repoProyectoUsuario;
        private readonly ITareaRepository _repoTarea;

        public TareaPermissionService(IProyectoUsuarioRepository repoProyectoUsuario, ITareaRepository repoTarea)
        {
            _repoProyectoUsuario = repoProyectoUsuario;
            _repoTarea = repoTarea;
        }
        // Comprueba que pertenezcas al proyecto, estés activo y tu rol.
        public async Task<bool> PuedePublicarTareasAsync(int proyectoId, int idPropia)
        {
            var usuarioPropio = await _repoProyectoUsuario.ObtenerUnUsuarioDeUnProyectoAsync(proyectoId, idPropia);

            return usuarioPropio is not null && usuarioPropio.Activo
            && (usuarioPropio.Rol == TaskFlow.Core.Enums.RolProyecto.Manager
            ||  usuarioPropio.Rol == TaskFlow.Core.Enums.RolProyecto.Administrador);
        }

        // Contiene las comprobaciones para poder modificar una tarea.
        public async Task<bool>PuedeModificarTareasAsync(int idPropia, Tarea tarea)
        {
            var usuarioPropio = await _repoProyectoUsuario.ObtenerUnUsuarioDeUnProyectoAsync(tarea.ProyectoId, idPropia);

            return usuarioPropio is not null && usuarioPropio.Activo
            && (usuarioPropio.Rol == TaskFlow.Core.Enums.RolProyecto.Manager
            || usuarioPropio.Rol == TaskFlow.Core.Enums.RolProyecto.Administrador
            || idPropia == tarea.AsignadoId
            || idPropia == tarea.CreadorId);
        }

        // Contiene las comprobaciones para poder modificar el estado de una tarea.
        public async Task<bool> PuedeModificarEstadoTareaAsync(int idPropia, Tarea tarea)
        {
            var usuarioPropio = await _repoProyectoUsuario.ObtenerUnUsuarioDeUnProyectoAsync(tarea.ProyectoId, idPropia);

            return usuarioPropio is not null && usuarioPropio.Activo
            && (usuarioPropio.Rol == TaskFlow.Core.Enums.RolProyecto.Manager
            || idPropia == tarea.AsignadoId
            || idPropia == tarea.CreadorId); 
        }
    }
}