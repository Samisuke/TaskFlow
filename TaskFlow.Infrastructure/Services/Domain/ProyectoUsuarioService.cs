using TaskFlow.Core.Services;
using TaskFlow.Core.Common;
using TaskFlow.Core.Models;
using TaskFlow.Core.Enums;
using TaskFlow.Core.Repositories;

namespace TaskFlow.Infrastructure.Services
{
    public class ProyectoUsuarioService : IProyectoUsuarioService
    {
        // Inyección del repositorio
        private readonly IProyectoUsuarioRepository _repoProyectoUsuario;
        private readonly IProyectoPermissionService _proyectoPermission;
        private readonly IHIstorialService _historialService;

        public ProyectoUsuarioService(IProyectoUsuarioRepository repoProyectoUsuario,
        IProyectoPermissionService proyectoPermission,
        IHIstorialService historialService
        )
        {
            _repoProyectoUsuario = repoProyectoUsuario;
            _proyectoPermission = proyectoPermission;
            _historialService = historialService;
        }

        // Peticiones GET
        // Todos los usuarios de un proyecto. Útil para ver de un vistazo quien trabaja en ese proyecto.
        public async Task<Result<IEnumerable<ProyectoUsuario>>> GetTodosLosUsuariosDeUnProyectoAsync(int idProyecto)
        {
            var usuarios = await _repoProyectoUsuario.ObtenerTodosUsuariosDeUnProyectoAsync(idProyecto);
            if (!usuarios.Any()) return Result<IEnumerable<ProyectoUsuario>>.Mal("El Proyecto aun no tiene usuarios.");

            return Result<IEnumerable<ProyectoUsuario>>.Bien(usuarios);
        }

        // Un usuario específico de un proyecto. Incluye la información de su perfil. Útil si te interesa ver más información de un usuario.
        public async Task<Result<ProyectoUsuario?>> GetUsuarioDeUnProyectoAsync(int idProyecto, int idUsuario)
        {
            var usuario = await _repoProyectoUsuario.ObtenerUnUsuarioDeUnProyectoAsync(idProyecto, idUsuario);
            if (usuario is null) return Result<ProyectoUsuario?>.Mal("El proyecto no tiene ese usuario asignado.");

            return Result<ProyectoUsuario?>.Bien(usuario);
        }
        
        //Petición POST
        // Añadir un usuario a un proyecto.
        public async Task<Result<ProyectoUsuario>> PostUsuarioAsync(
            int idPropia,
            int usuarioId,
            int proyectoId,
            // El usuario estará activo por defecto cuando lo añadas a un proyecto.
            RolProyecto rolUsuario
        )
        {
            // Comprobaciones.
            if (!await _proyectoPermission.PuedeAñadirPersonasAsync(proyectoId, usuarioId, idPropia)) return Result<ProyectoUsuario>.Mal("No puedes añadir este usuario al proyecto.");
            
            // Creación de usuario nuevo.
            var usuario = new ProyectoUsuario
            {
                UsuarioId = usuarioId,
                FechaIncorporacion = DateTime.UtcNow,
                ProyectoId = proyectoId,
                Rol = rolUsuario,
                Activo = true
            };

            // Base de datos.
            await _repoProyectoUsuario.CrearUsuarioAsync(usuario);
            var guardadoExistoso = await _repoProyectoUsuario.GuardarCambiosAsync();
            if (!guardadoExistoso) return Result<ProyectoUsuario>.Mal("Fallo inesperado al guardar los camibos. Inténtalo de nuevo más tarde.");

            await _historialService.AñadirPersonaProyectoAsync(proyectoId, idPropia);

            return Result<ProyectoUsuario>.Bien(usuario);
        }

        //Petición PATCH
        // Modificar el estado y el rol de un usuario de un proyecto. Solo puede hacerse si eres el Manager del proyecto.
        public async Task<Result<ProyectoUsuario>> PatchUsuarioAsync(
            int idPropia,
            int idUsuarioACambiar,
            int proyectoId,
            bool? activoUsuario,
            RolProyecto? rolUsuario
        )
        {
            int numeroCambios = 0;

            var usuario = await _repoProyectoUsuario.ObtenerUnUsuarioDeUnProyectoAsync(proyectoId, idUsuarioACambiar);
            if (usuario is null) return Result<ProyectoUsuario>.Mal("No se encuentra el usuario.");

            // Comprobaciones
            if (!await _proyectoPermission.PuedeModificarProyectoAsync(proyectoId, idPropia))

            // Realización de cambios.
            if (activoUsuario.HasValue)
            {
                usuario.Activo = activoUsuario.Value;
                numeroCambios += 1;    
            }
            if (rolUsuario.HasValue)
            {
                usuario.Rol = rolUsuario.Value;
                numeroCambios += 1;   
            }

            // Base de datos.
            if (numeroCambios == 0) return Result<ProyectoUsuario>.Mal("No se han detectado cambios para realizar.");
            var guardadoExistoso = await _repoProyectoUsuario.GuardarCambiosAsync();
            if (!guardadoExistoso) return Result<ProyectoUsuario>.Mal("Fallo inesperado al guardar los camibos. Inténtalo de nuevo más tarde.");

            await _historialService.ModificarPersonaProyectoAsync(proyectoId, idPropia);

            return Result<ProyectoUsuario>.Bien(usuario);
        }
    }
}