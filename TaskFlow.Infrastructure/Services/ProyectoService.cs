using Taskflow.Core.Repositories;
using TaskFlow.Core.Common;
using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Services;

namespace TaskFlow.Infrastructure.Services
{
    public class ProyectoService : IProyectoService
    {
        // Inyección del repositorio
        private readonly IProyectoRepository _repoProyecto;
        private readonly IUsuarioRepository _repoUsuario;

        public ProyectoService(IProyectoRepository repoProyecto, IUsuarioRepository repoUsuario)
        {
            _repoProyecto = repoProyecto;
            _repoUsuario = repoUsuario;
        }
        // Métodos GET
        public async Task<Result<IEnumerable<Proyecto>>> GetProyectosDeUnaPersonaAsync(int idUsuario)
        {
            var proyectos = await _repoProyecto.ObtenerProyectosDeUnUsuarioAsync(idUsuario);
            if (!proyectos.Any()) return Result<IEnumerable<Proyecto>>.Mal("ERROR. Este usuario no pertenece a ningún proyecto aun.");

            return Result<IEnumerable<Proyecto>>.Bien(proyectos);
        }

        public async Task<Result<IEnumerable<Proyecto>>> GetProyectosDeUnCreadorAsync(int idCreador)
        {
            var proyectos = await _repoProyecto.ObtenerProyectosDeUnCreadorAsync(idCreador);
            if (!proyectos.Any()) return Result<IEnumerable<Proyecto>>.Mal("ERROR. Este usuario no tiene proyectos creados.");

            return Result<IEnumerable<Proyecto>>.Bien(proyectos);      
        }

        // Métodos POST
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
            if (!guardadoExistoso) return Result<Proyecto>.Mal("ERROR. Fallo inesperado al guardar el usuario. Inténtalo de nuevo más tarde.");

            return Result<Proyecto>.Bien(proyecto);
        }

        // Métodos PATCH
        public async Task<Result<Proyecto>> PatchProyectoAsync(
        int idProyecto,
        string? nombreProyecto,
        string? descripcionProyecto,
        int? PropietarioId
        )
        {
            int numeroCambios = 0;

            var proyecto = await _repoProyecto.ObtenerProyectoPorIdAsync(idProyecto);
            if (proyecto is null) return Result<Proyecto>.Mal("ERROR. No se encuentra el proyecto.");

            if (nombreProyecto is not null)
            {
                proyecto.Nombre = nombreProyecto;
                numeroCambios += 1;
            } 
            if (descripcionProyecto is not null)
            {
                proyecto.Nombre = descripcionProyecto;
                numeroCambios += 1;
            } 
            if (PropietarioId.HasValue)
            {
                var usuario = await _repoUsuario.ObtenerUsuarioPorIdAsync(PropietarioId.Value);
                if (usuario is null) return Result<Proyecto>.Mal("ERROR. No se encuentra al usuario.");

                proyecto.PropietarioId = PropietarioId.Value;
                numeroCambios += 1;
            }

            if (numeroCambios == 0) return Result<Proyecto>.Mal("ERROR. No se han detectado cambios.");
            var guardadoExistoso = await _repoProyecto.GuardarCambiosASync();
            if (!guardadoExistoso) return Result<Proyecto>.Mal("ERROR. Fallo inesperado al guardar los cambios. Inténtalo de nuevo más tarde.");

            return Result<Proyecto>.Bien(proyecto);
        }
    }
}