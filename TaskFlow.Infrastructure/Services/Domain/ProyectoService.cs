using TaskFlow.Core.Repositories;
using TaskFlow.Core.Common;
using TaskFlow.Core.Models;
using TaskFlow.Core.Services;
using TaskFlow.Core.Enums;

namespace TaskFlow.Infrastructure.Services
{
    public class ProyectoService : IProyectoService
    {
        // Inyección del repositorio
        private readonly IProyectoRepository _repoProyecto;
        private readonly IUsuarioRepository _repoUsuario;
        private readonly IProyectoPermissionService _proyectoPermission;
        private readonly IHistorialService _historialService;

        public ProyectoService(
            IProyectoRepository repoProyecto,
            IUsuarioRepository repoUsuario,
            IProyectoPermissionService proyectoPermission,
            IHistorialService historialService
        )
        {
            _repoProyecto = repoProyecto;
            _repoUsuario = repoUsuario;
            _proyectoPermission = proyectoPermission;
            _historialService = historialService;
        }

        // Métodos GET
        // Obtener un proyecto por ID
        public async Task<Result<Proyecto>> GetProyectoPorIdAsync(int id)
        {
            var proyecto = await _repoProyecto.ObtenerProyectoPorIdAsync(id);
            if (proyecto is null) return Result<Proyecto>.Mal("No existe el proyecto,");

            return Result<Proyecto>.Bien(proyecto);
        }

        // Obtener proyectos a los que pertenece una persona. Útil para ver los proyectos de un contácto.
        public async Task<Result<IEnumerable<Proyecto>>> GetProyectosDeUnaPersonaAsync(int usuarioId)
        {
            var proyectos = await _repoProyecto.ObtenerProyectosDeUnUsuarioAsync(usuarioId);
            if (!proyectos.Any()) return Result<IEnumerable<Proyecto>>.Mal("Este usuario no pertenece a ningún proyecto aun.");

            return Result<IEnumerable<Proyecto>>.Bien(proyectos);
        }

        // Obtener los proyectos creados por una persona concreta. Útil para saber los proyectos de los que eres dueño.
        public async Task<Result<IEnumerable<Proyecto>>> GetProyectosDeUnCreadorAsync(int creadorId)
        {
            var proyectos = await _repoProyecto.ObtenerProyectosDeUnCreadorAsync(creadorId);
            if (!proyectos.Any()) return Result<IEnumerable<Proyecto>>.Mal("Este usuario no tiene proyectos creados.");

            return Result<IEnumerable<Proyecto>>.Bien(proyectos);      
        }

        // Métodos POST
        // Crear un proyecto nuevo. Hardcoded que el proyecto solo lo puedas crear siendo tu el propietario.
        public async Task<Result<Proyecto>> PostProyectoAsync(
            string nombreProyecto,
            string descripcionProyecto,
            int propietarioId
        )
        {
            var proyecto = new Proyecto
            {
                Nombre = nombreProyecto,
                Descripcion = descripcionProyecto,
                FechaCreacion = DateTime.UtcNow,
                PropietarioId = propietarioId
            };

            await _repoProyecto.CrearProyectoAsync(proyecto);
            var guardadoExistoso = await _repoProyecto.GuardarCambiosASync();
            if (!guardadoExistoso) return Result<Proyecto>.Mal("Fallo inesperado al guardar el usuario. Inténtalo de nuevo más tarde.");

            return Result<Proyecto>.Bien(proyecto);
        }

        // Métodos PATCH
        // Modificaciones del proyecto generales.
        public async Task<Result<Proyecto>> PatchProyectoAsync(
            int propiaId,
            int proyectoId,
            string? nombreProyecto,
            string? descripcionProyecto
        )
        {
            int numeroCambios = 0;

            var proyecto = await _repoProyecto.ObtenerProyectoPorIdAsync(proyectoId);
            if (proyecto is null) return Result<Proyecto>.Mal("No se encuentra el proyecto.");

            // Comprobaciones
            if (!await _proyectoPermission.PuedeModificarProyectoAsync(proyecto.Id, propiaId)) return Result<Proyecto>.Mal("no puedes modificar el proyecto.");
            
            if (nombreProyecto is not null)
            {
                proyecto.Nombre = nombreProyecto;
                numeroCambios += 1;
            } 
            if (descripcionProyecto is not null)
            {
                proyecto.Descripcion = descripcionProyecto;
                numeroCambios += 1;
            } 

            if (numeroCambios == 0) return Result<Proyecto>.Mal("No se han detectado cambios.");
            var guardadoExistoso = await _repoProyecto.GuardarCambiosASync();
            if (!guardadoExistoso) return Result<Proyecto>.Mal("Fallo inesperado al guardar los cambios. Inténtalo de nuevo más tarde.");
            await _historialService.ModificarProyectoAsync(proyecto, propiaId);

            return Result<Proyecto>.Bien(proyecto);
        }

        // Pasar la posesión del proyecto a otra persona.
        // Separado del patch normal porque requiere permisos distintos por reglas de negocio.
        public async Task<Result<Proyecto>> PatchDueñoProyectoAsync(
            int propiaId,
            int proyectoId,
            int propietarioNuevoId
        )
        {
            var proyecto = await _repoProyecto.ObtenerProyectoPorIdAsync(proyectoId);
            var usuarioNuevo = await _repoUsuario.ObtenerUsuarioPorIdAsync(propietarioNuevoId);

            // Comprobaciones de existencia
            if (proyecto is null) return Result<Proyecto>.Mal("No se encuentra el proyecto para transferir.");            
            if (usuarioNuevo is null) return Result<Proyecto>.Mal("No se encuentra la persona a la que quieres transferir el proyecto.");

            // Comprobaciones de proyecto.
            if (!await _proyectoPermission.PuedesTransferirProyectoAsync(proyecto, usuarioNuevo, propiaId)) return Result<Proyecto>.Mal("No puedes transferir el proyecto a esta persona.");

            // Cambiamos los roles del nuevo propietario y del antiguo.
            var propietarioActual = proyecto.Usuarios
                .First(x => x.UsuarioId == proyectoId);
            var nuevoPropietario = proyecto.Usuarios
                .First(x => x.UsuarioId == propietarioNuevoId);

            propietarioActual.Rol = RolProyecto.Administrador;
            nuevoPropietario.Rol = RolProyecto.Manager;

            // Cambiamos el ID en el FK.
            proyecto.PropietarioId = propietarioNuevoId;           
            
            // Base de datos
            var guardadoExistoso = await _repoProyecto.GuardarCambiosASync();
            if (!guardadoExistoso) return Result<Proyecto>.Mal("ERROR. Fallo inesperado al guardar los cambios. Inténtalo de nuevo más tarde.");

            await _historialService.ModificarDueñoProyectoAsync(proyecto, propiaId);

            return Result<Proyecto>.Bien(proyecto);
        }
    }
}