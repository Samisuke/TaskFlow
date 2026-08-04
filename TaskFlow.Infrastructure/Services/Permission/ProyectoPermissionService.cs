using TaskFlow.Core.Repositories;
using TaskFlow.Core.Models;
using TaskFlow.Core.Services;

namespace TaskFlow.Infrastructure.Services
{
    public class ProyectoPermissionService : IProyectoPermissionService
    {
        // Inyección del repositorio
        private readonly IProyectoUsuarioRepository _repoProyectoUsuario;
        private readonly IUsuarioRepository _repoUsuario;

        public ProyectoPermissionService(
            IProyectoUsuarioRepository repoProyectoUsuario,
            IUsuarioRepository repoUsuario
        )
        {
            _repoProyectoUsuario = repoProyectoUsuario;
            _repoUsuario = repoUsuario;
        }

        // Comprueba que pertenezcas al proyecto y estés activo.
        public async Task<bool> EsMiembroActivoAsync(int idProyecto, int idPropia)
        {
            var proyectoUsuario = await _repoProyectoUsuario.ObtenerUnUsuarioDeUnProyectoAsync(idProyecto, idPropia);
            return proyectoUsuario is not null && proyectoUsuario.Activo;
        }

        // Contiene las comprobaciones para poder modificar un proyecto.
        public async Task<bool>PuedeModificarProyectoAsync(int idProyecto, int idPropia)
        {
            var proyectoUsuario = await _repoProyectoUsuario.ObtenerUnUsuarioDeUnProyectoAsync(idProyecto, idPropia);
            
            return proyectoUsuario is not null 
            && proyectoUsuario.Activo
            && proyectoUsuario.Rol == TaskFlow.Core.Enums.RolProyecto.Manager;
        }

        // Contiene las comprobaciones para poder transferir un proyecto.
        public async Task<bool> PuedesTransferirProyectoAsync(Proyecto proyecto, Usuario usuarioNuevo, int idPropia)
        {
            // Comprobación: si no eres el dueño, no puedes pasar la posesión del proyecto.
            if (proyecto.PropietarioId != idPropia) return false;

            // Comprobación: Si el proyecto es tuyo no puedes pasartelo a ti mismo.
            if (proyecto.PropietarioId == usuarioNuevo.Id) return false;
                    
            // Comprobación: Si el nuevo propietario no pertenece al proyecto o no está activo, no puedes pasarlo.
            if (!await EsMiembroActivoAsync(proyecto.Id, usuarioNuevo.Id)) return false;

            // Comprobación: Si el miembro no está activo en la aplicación, no peudes pasarlo.
            if (!usuarioNuevo.Activo) return false;

            return true;
        }

        // Contiene las comprobaciones para poder añadir un usuario a un proyecto
        public async  Task<bool> PuedeAñadirPersonasAsync(int proyectoId, int nuevoUsuarioId, int idPropia)
        {
            var usuarioPropio = await _repoProyectoUsuario.ObtenerUnUsuarioDeUnProyectoAsync(proyectoId, idPropia);
            if (usuarioPropio is null) return false;

            var nuevoUsuario = await _repoUsuario.ObtenerUsuarioPorIdAsync(nuevoUsuarioId);
            if (nuevoUsuario is null) return false;

            // Comprobación de rol.
            if (usuarioPropio.Rol != TaskFlow.Core.Enums.RolProyecto.Manager && usuarioPropio.Rol != TaskFlow.Core.Enums.RolProyecto.Administrador) return false;

            // Comprobación de inactividad.
            if (!nuevoUsuario.Activo) return false;

            // Comprobación de pertenencia
            if (await EsMiembroActivoAsync(proyectoId, idPropia)) return false;

            return true;
        }
    }
}