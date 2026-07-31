using Taskflow.Core.Repositories;
using Taskflow.Core.Services;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Models;

namespace Taskflow.Infrastructure.Services
{
    public class ProyectoPermissionService : IProyectoPermissionService
    {
        // Inyección del repositorio
        private readonly IProyectoUsuarioRepository _repoProyectoUsuario;

        public ProyectoPermissionService(IProyectoUsuarioRepository repoProyectoUsuario)
        {
            _repoProyectoUsuario = repoProyectoUsuario;
        }

        // Comprueba que pertenezcas al proyecto y estés activo.
        public async Task<bool> EsMiembroActivoAsync(int idProyecto, int idPropia)
        {
            var proyectoUsuario = await _repoProyectoUsuario.ObtenerUnUsuarioDeUnProyectoAsync(idProyecto, idPropia);
            return proyectoUsuario is not null && proyectoUsuario.Activo;
        }

        // Contiene las comprobaciones para poder transferir un proyecto
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
    }
}