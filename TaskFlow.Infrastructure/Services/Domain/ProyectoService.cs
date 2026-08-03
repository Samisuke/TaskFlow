using Taskflow.Core.Repositories;
using TaskFlow.Core.Common;
using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Services;
using TaskFlow.Core.Enums;
using Taskflow.Core.Services;

namespace TaskFlow.Infrastructure.Services
{
    public class ProyectoService : IProyectoService
    {
        // Inyección del repositorio
        private readonly IProyectoRepository _repoProyecto;
        private readonly IUsuarioRepository _repoUsuario;
      private readonly IProyectoPermissionService _proyectoPermission;

        public ProyectoService(
        IProyectoRepository repoProyecto,
        IUsuarioRepository repoUsuario,
        IProyectoPermissionService proyectoPermission)
        {
            _repoProyecto = repoProyecto;
            _repoUsuario = repoUsuario;
            _proyectoPermission = proyectoPermission;
        }

        // Métodos GET
        // Obtener proyectos a los que pertenece una persona. Útil para ver tus propios proyectos.
        public async Task<Result<IEnumerable<Proyecto>>> GetProyectosDeUnaPersonaAsync(int idUsuario)
        {
            // Sacar los proyectos del usuario
            var proyectos = await _repoProyecto.ObtenerProyectosDeUnUsuarioAsync(idUsuario);
            if (!proyectos.Any()) return Result<IEnumerable<Proyecto>>.Mal("ERROR. Este usuario no pertenece a ningún proyecto aun.");

            return Result<IEnumerable<Proyecto>>.Bien(proyectos);
        }

        // Obtener los proyectos creados por una persona concreta. Útil para saber los proyectos de los que eres dueño.
        public async Task<Result<IEnumerable<Proyecto>>> GetProyectosDeUnCreadorAsync(int idCreador)
        {
            // Sacar los proyectos del usuario
            var proyectos = await _repoProyecto.ObtenerProyectosDeUnCreadorAsync(idCreador);
            if (!proyectos.Any()) return Result<IEnumerable<Proyecto>>.Mal("ERROR. Este usuario no tiene proyectos creados.");

            return Result<IEnumerable<Proyecto>>.Bien(proyectos);      
        }

        // Métodos POST
        // Crear un proyecto nuevo. Hardcoded que el proyecto solo lo puedas crear siendo tu el propietario.
        public async Task<Result<Proyecto>> PostProyectoAsync(
        string nombreProyecto,
        string descripcionProyecto,
        int PropietarioId
        )
        {
            var proyecto = new Proyecto
            {
                Nombre = nombreProyecto,
                Descripcion = descripcionProyecto,
                FechaCreacion = DateTime.UtcNow,
                PropietarioId = PropietarioId
            };

            await _repoProyecto.CrearProyectoAsync(proyecto);
            var guardadoExistoso = await _repoProyecto.GuardarCambiosASync();
            if (!guardadoExistoso) return Result<Proyecto>.Mal("Fallo inesperado al guardar el usuario. Inténtalo de nuevo más tarde.");

            return Result<Proyecto>.Bien(proyecto);
        }

        // Métodos PATCH
        // Modificaciones del proyecto generales.
        public async Task<Result<Proyecto>> PatchProyectoAsync(
        int idPropia,
        int idProyecto,
        string? nombreProyecto,
        string? descripcionProyecto
        )
        {
            int numeroCambios = 0;

            var proyecto = await _repoProyecto.ObtenerProyectoPorIdAsync(idProyecto);
            if (proyecto is null) return Result<Proyecto>.Mal("No se encuentra el proyecto.");

            // Comprobaciones
            if (!await _proyectoPermission.PuedeModificarProyectoAsync(proyecto.Id, idPropia)) return Result<Proyecto>.Mal("no puedes modificar el proyecto.");
            
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

            return Result<Proyecto>.Bien(proyecto);
        }

        // Pasar la posesión del proyecto a otra persona. Contiene comprobaciones básicas.
        public async Task<Result<Proyecto>> PatchDueñoProyectoAsync(
        int idPropia,
        int idProyecto,
        int PropietarioNuevoId
        )
        {
            var proyecto = await _repoProyecto.ObtenerProyectoPorIdAsync(idProyecto);
            var usuarioNuevo = await _repoUsuario.ObtenerUsuarioPorIdAsync(PropietarioNuevoId);

            // Comprobaciones de existencia
            if (proyecto is null) return Result<Proyecto>.Mal("No se encuentra el proyecto para transferir.");            
            if (usuarioNuevo is null) return Result<Proyecto>.Mal("No se encuentra la persona a la que quieres transferir el proyecto.");

            // Comprobaciones de proyecto.
            if (!await _proyectoPermission.PuedesTransferirProyectoAsync(proyecto, usuarioNuevo, idPropia)) return Result<Proyecto>.Mal("No puedes transferir el proyecto a esta persona.");

            // Cambiamos los roles del nuevo propietario y del antiguo.
            var propietarioActual = proyecto.Usuarios
                .First(x => x.UsuarioId == idPropia);
            var nuevoPropietario = proyecto.Usuarios
                .First(x => x.UsuarioId == PropietarioNuevoId);

            propietarioActual.Rol = RolProyecto.Administrador;
            nuevoPropietario.Rol = RolProyecto.Manager;

            // Cambiamos el ID en el FK.
            proyecto.PropietarioId = PropietarioNuevoId;           
            
            // Base de datos
            var guardadoExistoso = await _repoProyecto.GuardarCambiosASync();
            if (!guardadoExistoso) return Result<Proyecto>.Mal("ERROR. Fallo inesperado al guardar los cambios. Inténtalo de nuevo más tarde.");

            return Result<Proyecto>.Bien(proyecto);
        }
    }
}