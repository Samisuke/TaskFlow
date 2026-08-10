using TaskFlow.Core.Services;
using TaskFlow.Core.Common;
using TaskFlow.Core.Models;
using TaskFlow.Core.Enums;
using TaskFlow.Core.Repositories;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Services
{
    public class ProyectoUsuarioService : IProyectoUsuarioService
    {
        // Inyección del repositorio
        private readonly IProyectoUsuarioRepository _repoProyectoUsuario;
        private readonly IProyectoPermissionService _proyectoPermission;
        private readonly IHistorialService _historialService;
        private readonly TaskFlowDbContext _context;

        public ProyectoUsuarioService(
            IProyectoUsuarioRepository repoProyectoUsuario,
            IProyectoPermissionService proyectoPermission,
            IHistorialService historialService,
            TaskFlowDbContext context
        )
        {
            _repoProyectoUsuario = repoProyectoUsuario;
            _proyectoPermission = proyectoPermission;
            _historialService = historialService;
            _context = context;
        }

        // Peticiones GET
        // Todos los usuarios de un proyecto. Útil para ver de un vistazo quien trabaja en ese proyecto.
        public async Task<Result<IEnumerable<ProyectoUsuario>>> GetTodosLosUsuariosDeUnProyectoAsync(int proyectoId)
        {
            var usuarios = await _repoProyectoUsuario.ObtenerTodosUsuariosDeUnProyectoAsync(proyectoId);
            if (!usuarios.Any()) return Result<IEnumerable<ProyectoUsuario>>.Mal("El Proyecto aun no tiene usuarios.");

            return Result<IEnumerable<ProyectoUsuario>>.Bien(usuarios);
        }

        // Un usuario específico de un proyecto. Incluye la información de su perfil. Útil si te interesa ver más información de un usuario.
        public async Task<Result<ProyectoUsuario?>> GetUsuarioDeUnProyectoAsync(int proyectoId, int usuarioId)
        {
            var usuario = await _repoProyectoUsuario.ObtenerUnUsuarioDeUnProyectoAsync(proyectoId, usuarioId);
            if (usuario is null) return Result<ProyectoUsuario?>.Mal("El proyecto no tiene ese usuario asignado.");

            return Result<ProyectoUsuario?>.Bien(usuario);
        }
        
        //Petición POST
        // Añadir un usuario a un proyecto.
        public async Task<Result<ProyectoUsuario>> PostUsuarioAsync(
            int propiaId,
            int usuarioId,
            int proyectoId,
            RolProyecto rolUsuario
        )
        {
            // Comprobaciones.
            if (!await _proyectoPermission.PuedeAñadirPersonasAsync(proyectoId, usuarioId, propiaId)) return Result<ProyectoUsuario>.Mal("No puedes añadir este usuario al proyecto.");
            
            // Transacción
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Creación de usuario nuevo.
                var usuario = new ProyectoUsuario
                {
                    UsuarioId = usuarioId,
                    FechaIncorporacion = DateTime.UtcNow,
                    ProyectoId = proyectoId,
                    Rol = rolUsuario,
                    Activo = true // El usuario se crea activo por defecto
                };

                // Base de datos.
                await _repoProyectoUsuario.CrearUsuarioAsync(usuario);
                await _historialService.AñadirPersonaProyectoAsync(proyectoId, propiaId);
                await _context.SaveChangesAsync();

                // Cmomit
                await transaction.CommitAsync();

                return Result<ProyectoUsuario>.Bien(usuario);               
            }

            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        //Petición PATCH
        // Modificar el estado y el rol de un usuario de un proyecto. Solo puede hacerse si eres el Manager del proyecto.
        public async Task<Result<ProyectoUsuario>> PatchUsuarioAsync(
            int propiaId,
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
            if (!await _proyectoPermission.PuedeModificarProyectoAsync(proyectoId, propiaId)) return Result<ProyectoUsuario>.Mal("No puedes modificar los usuarios de este proyecto.");

            // Transacción
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
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
                if (numeroCambios == 0) return Result<ProyectoUsuario>.Mal("No se encontraron cambios");
                await _historialService.ModificarPersonaProyectoAsync(proyectoId, propiaId);
                await _context.SaveChangesAsync();

                // Commit
                await transaction.CommitAsync();

                return Result<ProyectoUsuario>.Bien(usuario);              
            }

            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}